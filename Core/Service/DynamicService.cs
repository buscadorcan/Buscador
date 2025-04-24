using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;
using SharedApp.Dtos;

namespace Core.Service
{
    public class DynamicService : IDynamicService
    {
        private readonly IDynamicRepository _dynamicRepository;

        public DynamicService(IDynamicRepository dynamicRepository)
        {
            this._dynamicRepository = dynamicRepository;
        }
        public ONAConexion GetConexion(int idONA)
        {
           return _dynamicRepository.GetConexion(idONA);
        }

        public List<EsquemaVistaDto> GetListaValidacionEsquema(int idONA, int idEsquemaVista)
        {
            return _dynamicRepository.GetListaValidacionEsquema(idONA, idEsquemaVista);
        }

        public List<PropiedadesTablaDto> GetProperties(int idONA, string viewName)
        {
           return _dynamicRepository.GetProperties(idONA, viewName);
        }

        public List<PropiedadesTablaDto> GetValueColumna(int idONA, string valueColumn, string viewName)
        {
           return _dynamicRepository.GetValueColumna(idONA, valueColumn, viewName);
        }

        public List<string> GetViewNames(int idONA)
        {
           return _dynamicRepository.GetViewNames(idONA);
        }

        public Task<bool> MigrarConexionAsync(int idONA)
        {
           return _dynamicRepository.MigrarConexionAsync(idONA);
        }

        public bool TestDatabaseConnection(ONAConexion conexion)
        {
           return _dynamicRepository.TestDatabaseConnection(conexion);
        }
    }
}
