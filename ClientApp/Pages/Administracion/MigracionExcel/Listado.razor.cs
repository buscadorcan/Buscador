using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Dtos;
using System.Reflection.Metadata.Ecma335;

namespace ClientApp.Pages.Administracion.MigracionExcel
{
    /// <summary>
    /// Componente de listado de logs de migración de archivos Excel.
    /// Controla el acceso según el rol del usuario y permite visualizar registros de migración.
    /// </summary>
    public partial class Listado
    {
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

        protected override async Task OnInitializedAsync()
        {
            objEventTracking.CodigoHomologacionMenu = "/migracion-excel";
            objEventTracking.NombreAccion = "OnInitializedAsync";
            objEventTracking.NombreControl = "migracion-excel";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            var usuarioBaseDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_BaseDatos_Local);
            var usuarioOrigenDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_OrigenDatos_Local);
            var usuarioEstadoMigracion = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_EstadoMigracion_Local);
            var usuarioMigrar = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Migrar_Local);
            var rolRelacionado = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);

            isRolRead = rolRelacionado == "KEY_USER_READ";
            isRolOna = rolRelacionado == "KEY_USER_ONA";
            isRolAdmin = rolRelacionado == "KEY_USER_CAN";

            // Verificación de acceso
            if (!isRolAdmin && !isRolOna)
            {
                if (!isRolRead)
                {
                    if (usuarioMigrar != "S" ||
                        usuarioEstadoMigracion != "A" ||
                        (usuarioBaseDatos != "INACAL" && usuarioBaseDatos != "DTA") ||
                        usuarioOrigenDatos != "EXCEL")
                    {
                        navigationManager?.NavigateTo("/page-nodisponible");
                        return;
                    }
                }
                else
                {
                    navigationManager?.NavigateTo("/page-nodisponible");
                    return;
                }
            }

            // Carga de datos con validación
            if (iLogMigracionService != null)
            {
                listasHevd = await iLogMigracionService.GetLogMigracionesAsync() ?? new List<LogMigracionDto>();
            }

            // Ajusta la paginación si la lista está vacía o cambia
            if (listasHevd.Count > 0 && CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
        }
    }
}