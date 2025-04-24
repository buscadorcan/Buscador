using Core.Interfaces;
using Core.Services;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Service
{
    public class ONAConexionService: IONAConexionService
    {
        private readonly IONAConexionRepository _oNAConexionRepository;
        private readonly IJwtService _jwtService;

        public ONAConexionService(IONAConexionRepository oNAConexionRepository, IJwtService jwtService)
        {
            this._oNAConexionRepository = oNAConexionRepository;
            this._jwtService = jwtService;
        }

        public bool Create(ONAConexion data)
        {

            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            data.IdUserModifica = data.IdUserCreacion;
            data.FechaCreacion = DateTime.Now;
            data.FechaModifica = data.FechaCreacion;
            data.Estado = "A";

            return _oNAConexionRepository.Create(data);
        }

        public List<ONAConexion> FindAll()
        {
            return _oNAConexionRepository.FindAll();
        }

        public ONAConexion? FindById(int Id)
        {
            return _oNAConexionRepository.FindById(Id);
        }

        public ONAConexion? FindByIdONA(int IdONA)
        {
            return _oNAConexionRepository.FindByIdONA(IdONA);
        }

        public Task<ONAConexion?> FindByIdONAAsync(int IdONA)
        {
            return _oNAConexionRepository.FindByIdONAAsync(IdONA);
        }

        public List<ONAConexion> GetOnaConexionByOnaListAsync(int IdONA)
        {
            return _oNAConexionRepository.GetOnaConexionByOnaListAsync(IdONA);
        }

        public bool Update(ONAConexion data)
        {
            var userToken = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

            return _oNAConexionRepository.Update(data, userToken);
        }
    }
}
