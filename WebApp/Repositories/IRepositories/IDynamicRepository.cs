using Microsoft.EntityFrameworkCore;
using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IDynamicRepository
    {
        List<PropiedadesTablaDto> GetProperties(int idONA, string viewName);
        List<PropiedadesTablaDto> GetValueColumna(int idONA, string valueColumn, string viewName);
        List<string> GetViewNames(int idONA);
        List<EsquemaVistaDto> GetListaValidacionEsquema(int idONA, int idEsquemaVista);
        ONAConexion GetConexion(int idONA);
        bool TestDatabaseConnection(ONAConexion conexion);
        Task<bool> MigrarConexionAsync(int idONA);
    }
}
