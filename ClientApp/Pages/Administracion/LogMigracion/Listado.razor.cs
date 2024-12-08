using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.LogMigracion
{
    public partial class Listado
    {
        private Grid<LogMigracionDto>? grid;
        [Inject]
        private ILogMigracionService? iLogMigracionService { get; set; }
        private List<LogMigracionDto>? listasHevd = null;
        private async Task<GridDataProviderResult<LogMigracionDto>> LogMigracionDtoDataProvider(GridDataProviderRequest<LogMigracionDto> request)
        {
            if (listasHevd == null && iLogMigracionService != null)
            {
                listasHevd = await iLogMigracionService.GetLogMigracionesAsync();
            }
            return await Task.FromResult(request.ApplyTo(listasHevd ?? []));
        }
    }
}