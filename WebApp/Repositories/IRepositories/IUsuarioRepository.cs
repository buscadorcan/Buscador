using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IUsuarioRepository
  {
    Usuario? FindById(int idUsuario);
    bool Create(Usuario usuario);
    bool Update(Usuario usuario);
    bool IsUniqueUser(string usuario);
    ICollection<UsuarioDto> FindAll();
    UsuarioAutenticacionRespuestaDto Login(UsuarioAutenticacionDto usuarioAutenticacionDto);
    Task<bool> RecoverAsync(UsuarioRecuperacionDto usuarioRecuperacionDto);
  }
}
