using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.CamposHomologacion
{
    /// <summary>
    /// Componente de formulario para la gestión de homologaciones.
    /// Permite registrar y actualizar campos de homologación en la plataforma.
    /// </summary>
    public partial class Formulario
    {
        // Botón de guardar con animación de carga
        private Button saveButton = default!;
        // Objeto que almacena los datos de la homologación a registrar/actualizar
        private HomologacionDto homologacion = new HomologacionDto();
        // Objeto que almacena la homologación padre del grupo
        private HomologacionDto homologacionGrupo = new HomologacionDto();
        // Lista de filtros disponibles para la homologación
        private List<VwFiltroDto> filtros = new();
        // Servicio de homologación inyectado
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        // Servicio de catálogos inyectado
        [Inject]
        public ICatalogosService? iCatalogoService { get; set; }
        // Administrador de navegación inyectado
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        // ID de la homologación (puede ser nulo si se está creando una nueva)
        [Parameter]
        public int? Id { get; set; }
        // ID del grupo padre de la homologación (puede ser nulo)
        [Parameter]
        public int? IdPadre { get; set; }
        // Servicio de notificaciones Toast inyectado
        [Inject]
        public Services.ToastService? toastService { get; set; }
        // Servicio de búsqueda inyectado
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();
        // Servicio de almacenamiento local en el navegador
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }


        /// <summary>
        /// Método asincrónico que inicializa el formulario de homologación.
        /// Carga los filtros disponibles y los datos de la homologación si se está editando.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            // Obtener los filtros de la base de datos
            filtros = await iCatalogoService.GetFiltrosAsync();
            // Obtener la homologación padre (grupo)
            homologacionGrupo = await iHomologacionService.GetHomologacionAsync((int) IdPadre);
            if (Id > 0) {
                objEventTracking.CodigoHomologacionMenu = "/editar-campos-homologacion";
                objEventTracking.NombreAccion = "OnInitializedAsync";
                objEventTracking.NombreControl = "editar-campos-homologacion";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                homologacion = await iHomologacionService.GetHomologacionAsync(Id.Value);
            } else {

                objEventTracking.CodigoHomologacionMenu = "/nuevo-campos-homologacion";
                objEventTracking.NombreAccion = "OnInitializedAsync";
                objEventTracking.NombreControl = "nuevo-campos-homologacion";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                homologacion.IdHomologacionGrupo = IdPadre;
                homologacion.InfoExtraJson = "{}";
                homologacion.MascaraDato = "TEXTO";
                homologacion.CodigoHomologacion = "";
                homologacion.SiNoHayDato = "";
            }
        }


        /// <summary>
        /// Método que guarda o actualiza una homologación en la base de datos.
        /// </summary>
        private async Task GuardarHomologacion()
        {
            objEventTracking.CodigoHomologacionMenu = "/nuevo-campos-homologacion";
            objEventTracking.NombreAccion = "GuardarHomologacion";
            objEventTracking.NombreControl = "btnGuardar";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "{}";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            saveButton.ShowLoading("Guardando...");

            var result = await iHomologacionService.RegistrarOActualizar(homologacion);
            if (result.registroCorrecto)
            {
                toastService?.CreateToastMessage(ToastType.Success, "Registrado exitosamente");
                navigationManager?.NavigateTo("/campos-homologacion");
            }
            else
            {
                toastService?.CreateToastMessage(ToastType.Danger, "Debe llenar todos los campos");
            }

            saveButton.HideLoading();
        }

        /// <summary>
        /// Método que actualiza el valor de la máscara de datos cuando el usuario cambia la selección.
        /// </summary>
        /// <param name="mascaraDato">Valor seleccionado en el campo de máscara.</param>
        private void OnAutoCompleteChanged(string mascaraDato) {
            homologacion.MascaraDato = mascaraDato;
        }

        /// <summary>
        /// Método que actualiza el filtro seleccionado en la homologación.
        /// </summary>
        /// <param name="e">Evento de cambio en la selección.</param>
        private void ActualizarFiltro(ChangeEventArgs e)
        {
            // Obtener el valor seleccionado
            var selectedValue = e.Value?.ToString();

            // Si el valor es "Sin Filtro" (vacío), asignar null a la variable
            if (string.IsNullOrEmpty(selectedValue))
            {
                homologacion.IdHomologacionFiltro = null;
            }
            else
            {
                // Convertir el valor a int, si es válido
                homologacion.IdHomologacionFiltro = int.TryParse(selectedValue, out var valor) ? valor : null;
            }
        }

        /// <summary>
        /// Propiedad booleana vinculada al Switch para la indexación del campo.
        /// </summary>
        private bool isIndexar // Propiedad booleana vinculada al Switch
        {
            get => homologacion.Indexar == "S"; // Convertir "S" a true
            set => homologacion.Indexar = value ? "S" : "N"; // Convertir true a "S"
        }

        /// <summary>
        /// Propiedad booleana vinculada al Switch para la visibilidad del campo.
        /// </summary>
        private bool isMostrar 
        {
            get => homologacion.Mostrar == "S"; // Convertir "S" a true
            set => homologacion.Mostrar = value ? "S" : "N"; // Convertir true a "S"
        }
    }
}