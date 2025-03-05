using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Bitacoras
{
    public partial class Listado
    {
        private EsquemaDto? esquemaSelected;
        private OnaDto? onaSelected;
        private Button buscarButton = default!;
        private DateTime? fechaInicio { get; set; } = DateTime.Today;
        private DateTime? fechaFin { get; set; } = DateTime.Today;
        private int? selectedOna { get; set; }

        [Inject]
        public Services.ToastService? toastService { get; set; }
        [Inject]
        public IONAService? iONAservice { get; set; }
        /// <summary>
        /// Servicio de navegación para redirigir a otras páginas.
        /// </summary>
        [Inject] public NavigationManager? navigationManager { get; set; }
        /// <summary>
        /// Servicio de almacenamiento local en el navegador.
        /// </summary>
        [Inject] ILocalStorageService iLocalStorageService { get; set; }
        /// <summary>
        /// Servicio de migración de archivos Excel.
        /// </summary>
        [Inject] private IMigracionExcelService? iMigracionExcelService { get; set; }
        /// <summary>
        /// Servicio de logs de migración.
        /// </summary>
        [Inject] private ILogMigracionService? iLogMigracionService { get; set; }
        // Componente de la grilla para mostrar los registros de migración
        private Grid<LogMigracionDto>? grid;
        // Variables de control de acceso según el rol del usuario
        private bool accessMigration;
        private bool isRolRead;
        private bool isRolOna;
        private bool isRolAdmin;
        /// <summary>
        /// Servicio de búsqueda y registro de eventos.
        /// </summary>
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        // Objeto para el seguimiento de eventos
        private EventTrackingDto objEventTracking { get; set; } = new();
        // Lista que almacena los registros de logs de migración
        private List<LogMigracionDto> listasHevd = new();
        // Parámetros para la paginación
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        /// <summary>
        /// Obtiene los elementos paginados para la grilla.
        /// </summary>
        private IEnumerable<LogMigracionDto> PaginatedItems => listasHevd
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        /// <summary>
        /// Calcula el número total de páginas basado en el número de registros.
        /// </summary>
        private int TotalPages => listasHevd.Count > 0 ? (int)Math.Ceiling((double)listasHevd.Count / PageSize) : 1;

        /// <summary>
        /// Indica si se puede retroceder a la página anterior.
        /// </summary>
        private bool CanGoPrevious => CurrentPage > 1;
        /// <summary>
        /// Indica si se puede avanzar a la siguiente página.
        /// </summary>
        private bool CanGoNext => CurrentPage < TotalPages;


        /// <summary>
        /// Cambia a la página anterior en la paginación.
        /// </summary>
        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        /// <summary>
        /// Cambia a la siguiente página en la paginación.
        /// </summary>
        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        /// <summary>
        /// Método asincrónico que se ejecuta al inicializar el componente.
        /// Carga la lista de logs de migración y controla el acceso según el rol del usuario.
        /// </summary>
        private List<OnaDto>? listaONAs;
       
        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/validacion";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "validacion";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            // Carga de datos con validación
            if (iLogMigracionService != null)
            {
                await LoadONAs();
            }

            // Ajusta la paginación si la lista está vacía o cambia
            if (listasHevd.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }

        private async Task LoadONAs()
        {
            if (iONAservice != null)
            {
                listaONAs = await iONAservice.GetONAsAsync();
            }
        }

        private async Task EliminarRegistro()
        {
            try
            {
                if (selectedOna == null)
                {
                    toastService?.CreateToastMessage(ToastType.Warning, "Debe seleccionar un ONA para eliminar.");
                    return;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void ActualizarFechas(ChangeEventArgs e, bool esInicio)
        {
            if (DateTime.TryParse(e.Value?.ToString(), out DateTime fechaSeleccionada))
            {
                if (esInicio)
                    fechaInicio = fechaSeleccionada;
                else
                    fechaFin = fechaSeleccionada;
            }
            StateHasChanged();
        }

        private async Task BuscarDatos()
        {
            try
            {
                objEventTracking.CodigoHomologacionMenu = "/bitacora";
                objEventTracking.NombreAccion = "BuscarDatos";
                objEventTracking.NombreControl = "btnGuardar";
                objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
                objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
                objEventTracking.ParametroJson = "{}";
                objEventTracking.UbicacionJson = "";
                await iBusquedaService.AddEventTrackingAsync(objEventTracking);

                buscarButton.ShowLoading("Buscando...");

                if (fechaInicio == null || fechaFin == null)
                {
                    toastService?.CreateToastMessage(ToastType.Warning, "Debe seleccionar las fechas.");
                    return;
                }

                if (fechaInicio > fechaFin)
                {
                    toastService?.CreateToastMessage(ToastType.Warning, "La fecha de inicio no puede ser mayor que la fecha fin.");
                    return;
                }

            }
            catch (Exception ex)
            {
                buscarButton.HideLoading();
            }

        }

    }
}
