using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;
using System.Reflection.Metadata.Ecma335;

namespace ClientApp.Pages.Administracion.MigracionExcel
{
    //public partial class Listado
    //{
    //    [Inject]
    //    public NavigationManager? navigationManager { get; set; }
    //    [Inject]
    //    ILocalStorageService iLocalStorageService { get; set; }
    //    private Grid<LogMigracionDto>? grid;
    //    [Inject]
    //    private IMigracionExcelService? iMigracionExcelService { get; set; }
    //    [Inject]
    //    private ILogMigracionService? iLogMigracionService { get; set; }
    //    //private List<LogMigracionDto>? listasHevd = null;

    //    private bool accessMigration;
    //    private bool isRolRead;
    //    private bool isRolOna;
    //    private bool isRolAdmin;
    //    private List<LogMigracionDto> listasHevd = new();
    //    protected override async Task OnInitializedAsync()
    //    {
    //        var usuarioBaseDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_BaseDatos_Local);
    //        var usuarioOrigenDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_OrigenDatos_Local);
    //        var usuarioEstadoMigracion = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_EstadoMigracion_Local);
    //        var usuarioMigrar = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Migrar_Local);
    //        var rolRelacionado = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);

    //        isRolRead = rolRelacionado == "KEY_USER_READ";
    //        isRolOna = rolRelacionado == "KEY_USER_ONA";
    //        isRolAdmin = rolRelacionado == "KEY_USER_CAN"; 
    //        if (!isRolAdmin && !isRolOna)
    //        {
    //            if (!isRolRead)
    //            {
    //                if (usuarioMigrar != "S" ||
    //                                 usuarioEstadoMigracion != "A" ||
    //                                 (usuarioBaseDatos != "INACAL" && usuarioBaseDatos != "DTA") ||
    //                                 usuarioOrigenDatos != "EXCEL")
    //                {
    //                    navigationManager?.NavigateTo("/page-nodisponible");
    //                    return;
    //                }
    //            }
    //            else
    //            {
    //                navigationManager?.NavigateTo("/page-nodisponible");
    //                return;
    //            }

    //        }

    //        if (listasHevd != null && iLogMigracionService != null)
    //        {
    //            listasHevd = await iLogMigracionService.GetLogMigracionesAsync() ?? new List<LogMigracionDto>();
    //        }

    //    }

    //}
    public partial class Listado
    {
        [Inject] public NavigationManager? navigationManager { get; set; }
        [Inject] ILocalStorageService iLocalStorageService { get; set; }
        [Inject] private IMigracionExcelService? iMigracionExcelService { get; set; }
        [Inject] private ILogMigracionService? iLogMigracionService { get; set; }

        private Grid<LogMigracionDto>? grid;
        private bool accessMigration;
        private bool isRolRead;
        private bool isRolOna;
        private bool isRolAdmin;
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        private List<LogMigracionDto> listasHevd = new();
        private int PageSize = 10; // Cantidad de registros por página
        private int CurrentPage = 1;

        private IEnumerable<LogMigracionDto> PaginatedItems => listasHevd
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize);

        private int TotalPages => listasHevd.Count > 0 ? (int)Math.Ceiling((double)listasHevd.Count / PageSize) : 1;

        private bool CanGoPrevious => CurrentPage > 1;
        private bool CanGoNext => CurrentPage < TotalPages;

        private void PreviousPage()
        {
            if (CanGoPrevious)
            {
                CurrentPage--;
            }
        }

        private void NextPage()
        {
            if (CanGoNext)
            {
                CurrentPage++;
            }
        }

        protected override async Task OnInitializedAsync()
        {
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