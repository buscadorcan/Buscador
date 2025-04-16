using SharedApp.Dtos;

namespace Infractruture.Interfaces
{
    public interface IReporteService
    {
        //titulo
        Task<HomologacionDto> findByVista(string codigoHomologacion);

        //usuario
        Task<T?> GetVwAcreditacionOnaAsync<T>(string endpoint);
        Task<T?> GetVwAcreditacionEsquemaAsync<T>(string endpoint);
        Task<T?> GetVwEstadoEsquemaAsync<T>(string endpoint);
        Task<T?> GetVwOecPaisAsync<T>(string endpoint);
        Task<T?> GetVwEsquemaPaisAsync<T>(string endpoint);
        Task<T?> GetVwOecFechaAsync<T>(string endpoint);

        //read
        Task<T?> GetVwProfesionalCalificadoAsync<T>(string endpoint);
        Task<T?> GetVwProfesionalOnaAsync<T>(string endpoint);
        Task<T?> GetVwProfesionalEsquemaAsync<T>(string endpoint);
        Task<T?> GetVwProfesionalFechaAsync<T>(string endpoint);
        Task<T?> GetVwCalificaUbicacionAsync<T>(string endpoint);

        //can
        Task<T?> GetVwBusquedaFechaAsync<T>(string endpoint);
        Task<T?> GetVwBusquedaFiltroAsync<T>(string endpoint);
        Task<T?> GetVwBusquedaUbicacionAsync<T>(string endpoint);
        Task<T?> GetVwActualizacionONAAsync<T>(string endpoint);

        //ona
        Task<T?> GetVwOrganismoRegistradoAsync<T>(string endpoint);
        Task<T?> GetVwOrganizacionEsquemaAsync<T>(string endpoint);
        Task<T?> GetVwOrganismoActividadAsync<T>(string endpoint);

    }
}
