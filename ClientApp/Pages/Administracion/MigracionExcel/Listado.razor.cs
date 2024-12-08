using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.MigracionExcel
{
    public partial class Listado
    {
        private Grid<MigracionExcelDto>? grid;
        [Inject]
        private IMigracionExcelService? iMigracionExcelService { get; set; }
        private List<MigracionExcelDto>? listasHevd = null;
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