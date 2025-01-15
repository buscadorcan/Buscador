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
        private Grid<MigracionExcelDto>? grid;
        [Inject]
        private IMigracionExcelService? iMigracionExcelService { get; set; }
        private List<MigracionExcelDto>? listasHevd = null;
        private bool accessMigration;
        protected override async Task OnInitializedAsync()
        {
            var usuarioBaseDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_BaseDatos_Local);
            var usuarioOrigenDatos = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_OrigenDatos_Local);
            var usuarioEstadoMigracion = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_EstadoMigracion_Local);
            var usuarioMigrar = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Migrar_Local);

            if (usuarioMigrar != "S")
            {
                navigationManager?.NavigateTo("/page-nodisponible");
            }
            else
            {
                if (usuarioEstadoMigracion != "A")
                {
                    navigationManager?.NavigateTo("/page-nodisponible");
                }
                else
                {
                    if (usuarioBaseDatos != "INACAL" && usuarioBaseDatos != "DTA")
                    {
                        navigationManager?.NavigateTo("/page-nodisponible");
                    }
                    else
                    {
                        if (usuarioOrigenDatos != "EXCEL")
                        {
                            navigationManager?.NavigateTo("/page-nodisponible");
                        }
                    }
                }
            }
            
        }
        private async Task<GridDataProviderResult<MigracionExcelDto>> MigracionExcelDtoDataProvider(GridDataProviderRequest<MigracionExcelDto> request)
        {
            if (listasHevd == null && iMigracionExcelService != null)
            {
                listasHevd = await iMigracionExcelService.GetMigracionExcelsAsync();
            }
            return await Task.FromResult(request.ApplyTo(listasHevd ?? []));
        }
    }
}