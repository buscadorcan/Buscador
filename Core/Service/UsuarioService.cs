using Core.Interfaces;
using Core.Services;
using DataAccess.Interfaces;
using DataAccess.Models;
using SharedApp.Dtos;

namespace Core.Service
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IHashService _hashService;
        private readonly IJwtService _jwtService;

        public UsuarioService(IUsuarioRepository usuarioRepository, 
                              IHashService hashService, 
                              IJwtService jwtService)
        {
            this._usuarioRepository = usuarioRepository;
            this._hashService = hashService;
            this._jwtService = jwtService;
        }

        public Result<bool> ChangePasswd(string clave, string claveNueva)
        {
            var actual = _hashService.GenerateHash(clave);
            var nueva = _hashService.GenerateHash(claveNueva);
            var idUsuario = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "0");

            return _usuarioRepository.ChangePasswd(clave, claveNueva, idUsuario, nueva);
        }

        public bool Create(Usuario usuario)
        {
            var clave = usuario.Clave;
            usuario.Clave = _hashService.GenerateHash(clave);
            usuario.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            usuario.IdUserModifica = usuario.IdUserCreacion;
            usuario.Estado = "A";

            return _usuarioRepository.Create(usuario);
        }

        public ICollection<UsuarioDto> FindAll()
        {
           return _usuarioRepository.FindAll();
        }

        public Usuario? FindByEmail(string email)
        {
            return _usuarioRepository.FindByEmail(email);
        }

        public Usuario? FindById(int idUsuario)
        {
            return _usuarioRepository.FindById(idUsuario);
        }

        public bool IsUniqueUser(string usuario)
        {
            return _usuarioRepository.IsUniqueUser(usuario);
        }

        public bool Update(Usuario usuario)
        {
           return _usuarioRepository.Update(usuario);
        }
    }
}
