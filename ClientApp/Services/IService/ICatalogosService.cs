namespace ClientApp.Services.IService
{
    public interface ICatalogosService
    {
        Task<T?> GetHomologacionAsync<T>(string endpoint);
        Task<T?> GetHomologacionDetalleAsync<T>(string endpoint, int IdHomologacion);
    }
}