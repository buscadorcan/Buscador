namespace ClientApp.Services.IService
{
    public interface IReporteService
    {
        Task<T?> GetVwAcreditacionOnaAsync<T>(string endpoint);
        Task<T?> GetVwAcreditacionEsquemaAsync<T>(string endpoint);
        Task<T?> GetVwEstadoEsquemaAsync<T>(string endpoint);
        Task<T?> GetVwOecPaisAsync<T>(string endpoint);
        Task<T?> GetVwEsquemaPaisAsync<T>(string endpoint);
        Task<T?> GetVwOecFechaAsync<T>(string endpoint);
    }
}
