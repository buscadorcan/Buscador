using Core.Interfaces;
using Core.Services;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Service
{
    public class UsuarioEndpontService : IUsuarioEndpontService
    {
        private readonly IUsuarioEndpointRepository _usuarioEndpointRepository;
        private readonly IJwtService _jwtService;

        public UsuarioEndpontService(IUsuarioEndpointRepository usuarioEndpointRepository, IJwtService jwtService)
        {
            this._usuarioEndpointRepository = usuarioEndpointRepository;
            this._jwtService = jwtService;
        }

        public bool Create(UsuarioEndpoint usuarioEndpoint)
        {
            usuarioEndpoint.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            return _usuarioEndpointRepository.Create(usuarioEndpoint);
        }

        public ICollection<UsuarioEndpoint> FindAll()
        {
           return _usuarioEndpointRepository.FindAll();
        }

        public UsuarioEndpoint? FindByEndpointId(int endpointId)
        {
            return _usuarioEndpointRepository.FindByEndpointId(endpointId);
        }
    }
}
