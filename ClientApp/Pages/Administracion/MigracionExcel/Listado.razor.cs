using BlazorBootstrap;
using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;
using System.Reflection.Metadata.Ecma335;

namespace ClientApp.Pages.Administracion.MigracionExcel
{
    public partial class Listado
    {
        [Inject]
        public NavigationManager? navigationManager { get; set; }
        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        private Grid<LogMigracionDto>? grid;
        [Inject]
        private IMigracionExcelService? iMigracionExcelService { get; set; }
        [Inject]
        private ILogMigracionService? iLogMigracionService { get; set; }
        //private List<MigracionExcelDto>? listasHevd = null;
        private List<LogMigracionDto>? listasHevd = null;

        private bool accessMigration;
        private bool isRolRead;
        private bool isRolAdmin;
        protected override async Task OnInitializedAsync()
        {
            var usuarioBaseDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_BaseDatos_Local);
            var usuarioOrigenDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_OrigenDatos_Local);
            var usuarioEstadoMigracion = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_EstadoMigracion_Local);
            var usuarioMigrar = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Migrar_Local);
            var rolRelacionado = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);

            isRolRead = rolRelacionado == "KEY_USER_READ";
            isRolAdmin = rolRelacionado == "KEY_USER_CAN"; 
            if (!isRolAdmin)
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
            

        }
        private async Task<GridDataProviderResult<LogMigracionDto>> LogMigracionDtoDataProvider(GridDataProviderRequest<LogMigracionDto> request)
        {
            if (listasHevd == null && iLogMigracionService != null)
            {
                listasHevd = await iLogMigracionService.GetLogMigracionesAsync();
            }
            return await Task.FromResult(request.ApplyTo(listasHevd ?? []));
        }
        //private async Task<GridDataProviderResult<MigracionExcelDto>> MigracionExcelDtoDataProvider(GridDataProviderRequest<MigracionExcelDto> request)
        //{
        //    if (listasHevd == null && iMigracionExcelService != null)
        //    {
        //        listasHevd = await iMigracionExcelService.GetMigracionExcelsAsync();
        //    }
        //    return await Task.FromResult(request.ApplyTo(listasHevd ?? []));
        //}
    }
}