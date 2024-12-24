using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Conexion
{
    public partial class Listado
    {
        private Grid<ONAConexionDto>? grid;
        [Inject]
        private IConexionService? iConexionService { get; set; }
        private List<ONAConexionDto>? listasHevd = null;
        private async Task<GridDataProviderResult<ONAConexionDto>> ConexionDtoDataProvider(GridDataProviderRequest<ONAConexionDto> request)
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
                    listasHevd = listasHevd.Where(c => c.IdONA != IdConexion).ToList();
                    await grid.RefreshDataAsync();
                }
            }
        }
    }
}