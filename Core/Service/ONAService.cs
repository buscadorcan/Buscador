using Core.Interfaces;
using Core.Services;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Service
{
    public class ONAService : IONAService
    {
        private readonly IONARepository oNARepository;
        private readonly IJwtService _jwtService;

        public ONAService(IONARepository oNARepository, IJwtService jwtService)
        {
            this.oNARepository = oNARepository;
            this._jwtService = jwtService;
        }
        public bool Create(ONA data)
        {
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            return oNARepository.Create(data);
        }

        public List<ONA> FindAll()
        {
            return oNARepository.FindAll();
        }

        public List<VwPais> FindAllPaises()
        {
            return oNARepository.FindAllPaises();
        }

        public ONA? FindById(int Id)
        {
            return oNARepository.FindById(Id);
        }

        public Task<ONA?> FindByIdAsync(int Id)
        {
            return oNARepository.FindByIdAsync(Id);
        }

        public ONA? FindBySiglas(string siglas)
        {
            return oNARepository.FindBySiglas(siglas);
        }

        public List<ONA> GetListByONAsAsync(int idOna)
        {
            return oNARepository.GetListByONAsAsync(idOna);
        }

        public bool Update(ONA data)
        {
            var userToken = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            return oNARepository.Update(data, userToken);
        }
    }
}
