using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Conexion
{
    public partial class Listado
    {
        private Grid<ConexionDto>? grid;
        [Inject]
        private IConexionService? iConexionService { get; set; }
        private List<ConexionDto>? listasHevd = null;
        private async Task<GridDataProviderResult<ConexionDto>> ConexionDtoDataProvider(GridDataProviderRequest<ConexionDto> request)
        {
            if (listasHevd == null && iConexionService != null)
            {
                listasHevd = await iConexionService.GetConexionsAsync();
            }
            return await Task.FromResult(request.ApplyTo(listasHevd ?? []));
        }
        private async Task OnDeleteClick(int IdConexion)
        {
            if (iConexionService != null && listasHevd != null && grid != null)
            {
                var respuesta = await iConexionService.EliminarConexion(IdConexion);
                if (respuesta.registroCorrecto) {
                    listasHevd = listasHevd.Where(c => c.IdConexion != IdConexion).ToList();
                    await grid.RefreshDataAsync();
                }
            }
        }
    }
}