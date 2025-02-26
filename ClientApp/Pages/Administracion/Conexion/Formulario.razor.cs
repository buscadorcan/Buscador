using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Conexion
{
    /// <summary>
    /// Página de formulario para la gestión de conexiones a diferentes motores de base de datos.
    /// Permite registrar o editar conexiones hacia servidores como: EXCEL, MSSQLSERVER, MYSQL, POSTGRESQL, SQLITE.
    /// </summary>
    public partial class Formulario
    {
        // Botón con animación de carga
        private Button saveButton = default!;
        /// <summary>
        /// ID de la conexión, nulo si es un nuevo registro.
        /// </summary>
        [Parameter]
        public int? Id { get; set; }
        // Servicio de gestión de conexiones
        [Inject]
        private IConexionService? service { get; set; }
        // Servicio para gestionar Organismos Nacionales de Acreditación (ONA)
        [Inject]
        private IONAService? iOnaService { get; set; }
        // Administrador de navegación inyectado
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        // Objeto que almacena la información de la conexión
        private ONAConexionDto conexion = new ONAConexionDto();
        // Servicio de homologación inyectado
        [Inject]
        public IHomologacionService? HomologacionService { get; set; }
        // Servicio de homologación inyectado (duplicado, se podría eliminar uno)
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }

        // Servicio de notificaciones Toast

        [Inject]
        public Services.ToastService? ToastService { get; set; }
        // Servicio de búsqueda inyectado
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Servicio de almacenamiento local
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();
        
        // Lista de organizaciones disponibles
        private List<OnaDto>? listaOrganizaciones = default;
        // Nombre de la homologación seleccionada
        private string? homologacionName;
        // Lista de homologaciones obtenidas desde la base de datos
        private List<HomologacionDto>? listaVwHomologacion;
        // Lista de homologaciones filtradas
        private IEnumerable<HomologacionDto>? lista = new List<HomologacionDto>();

        /// <summary>
        /// Método asincrónico que inicializa la página cargando las conexiones disponibles y organizaciones.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            if (iOnaService != null)
            {
                listaOrganizaciones = await iOnaService.GetONAsAsync();
            }

            if (listaVwHomologacion == null)
                listaVwHomologacion = await iHomologacionService.GetHomologacionsAsync();

            if (Id > 0 && service != null)
            {
                objEventTracking.NombrePagina = "/editar-conexion";
                objEventTracking.NombreAccion = "OnInitializedAsync";
                objEventTracking.NombreControl = "editar-conexion";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                conexion = await service.GetConexionAsync(Id.GetValueOrDefault());
            }
            else
            {
                objEventTracking.NombrePagina = "/nuevo-conexion";
                objEventTracking.NombreAccion = "OnInitializedAsync";
                objEventTracking.NombreControl = "nuevo-conexion";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);
            }
            
           
        }

        /// <summary>
        /// Método que guarda o actualiza una conexión en la base de datos.
        /// </summary>
        private async Task RegistrarConexion()
        {
            objEventTracking.NombrePagina = "/nuevo-conexion";
            objEventTracking.NombreAccion = "RegistrarConexion";
            objEventTracking.NombreControl = "btnGuardar";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            saveButton.ShowLoading("Guardando...");

            if (service != null)
            {
                try
                {
                    var result = await service.RegistrarOActualizar(conexion);
                    if (result.registroCorrecto)
                    {
                        //Mensaje de éxito
                        ToastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                        navigationManager?.NavigateTo("/conexion");
                    }
                    else
                    {
                        // Mensaje de error
                        ToastService?.CreateToastMessage(ToastType.Danger, "Error al registrar en el servidor");
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de errores
                    Console.WriteLine($"Error al registrar conexión: {ex.Message}");
                }
            }

            saveButton.HideLoading();
        }

        /// <summary>
        /// Método que actualiza la selección de organización en la conexión.
        /// </summary>
        /// <param name="_organizacionSelected">Identificador de la organización seleccionada.</param>
        private void CambiarSeleccionOrganizacion(string _organizacionSelected)
        {
            var conexion = _organizacionSelected;
        }
        private void CambiarSeleccionMotor(ChangeEventArgs e)
        {
            conexion.BaseDatos = e.Value?.ToString();
            conexion.OrigenDatos = e.Value?.ToString();
        }

        /// <summary>
        /// Método que proporciona datos de homologaciones para el AutoComplete.
        /// </summary>
        private async Task<AutoCompleteDataProviderResult<HomologacionDto>> VwHomologacionDataProvider(AutoCompleteDataProviderRequest<HomologacionDto> request)
        {
            if (listaVwHomologacion == null)
                listaVwHomologacion = await HomologacionService.GetHomologacionsAsync();
            // Devuelve una lista vacía si no hay datos.
            if (listaVwHomologacion == null || !listaVwHomologacion.Any())
            {
                return new AutoCompleteDataProviderResult<HomologacionDto>
                {
                    Data = new List<HomologacionDto>(),
                    TotalCount = 0
                };
            }

            // Aplica el filtro ingresado en el AutoComplete.
            var filtro = request.Filter.Value.ToLowerInvariant();
            var resultados = listaVwHomologacion
                .Where(h => string.IsNullOrEmpty(filtro) ||
                            (h.MostrarWeb?.ToLowerInvariant().Contains(filtro) ?? false))
                .OrderBy(h => h.MostrarWebOrden)
                .Take(10) //como utilizar Top 10 en consulta SQL
                .ToList();

            return new AutoCompleteDataProviderResult<HomologacionDto>
            {
                Data = resultados,
                TotalCount = resultados.Count
            };
        }

        /// <summary>
        /// Propiedad booleana vinculada al Switch para la opción de migración.
        /// </summary>
        private bool isMigrar // Propiedad booleana vinculada al Switch
        {
            get => conexion.Migrar == "S"; // Convertir "S" a true
            set => conexion.Migrar = value ? "S" : "N"; // Convertir true a "S"
        }
        /// <summary>
        /// Método que se ejecuta cuando se selecciona un valor en el AutoComplete.
        /// </summary>
        private void OnAutoCompleteChanged(HomologacionDto vwHomologacionSelected)
        {
            lista = lista?.Append(vwHomologacionSelected).ToList();
        }
        
    }
}