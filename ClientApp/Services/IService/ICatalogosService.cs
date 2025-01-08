using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService
{
    public interface ICatalogosService
    {
        Task<T?> GetHomologacionAsync<T>(string endpoint);
        Task<T?> GetFiltroDetalleAsync<T>(string endpoint, string CodigoHomologacion);
        Task<List<VwMenuDto>> GetMenusAsync();
        Task<List<VwFiltroDto>> GetFiltrosAsync();
    }
}