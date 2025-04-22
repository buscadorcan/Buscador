using SharedApp.Dtos;

namespace Infractruture.Interfaces
{
    public interface ICatalogosService
    {
        Task<T?> GetHomologacionAsync<T>(string endpoint);
        Task<T?> GetFiltroDetalleAsync<T>(string endpoint, string CodigoHomologacion);
        Task<List<VwMenuDto>> GetMenusAsync();
        Task<List<VwFiltroDto>> GetFiltrosAsync();
        Task<List<vwPanelONADto>> GetPanelOnaAsync();
        Task<List<vwONADto>> GetvwOnaAsync();
        Task<List<vwEsquemaOrganizaDto>> GetvwEsquemaOrganizaAsync();
        Task<Dictionary<string, List<vw_FiltrosAnidadosDto>>> GetFiltrosAnidadosAsync(List<FiltrosBusquedaSeleccionDto> filtros);
        Task<List<vw_FiltrosAnidadosDto>> ObtenerFiltrosAnidadosAllAsync();
    }
}