using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IUsuarioRepository
  {
    Usuario? FindById(int idUsuario);
    /// <summary>
    /// Finds a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user to find.</param>
    /// <returns>
    /// A <see cref="Usuario"/> object representing the user if found; otherwise, <c>null</c>.
    /// </returns>
    Usuario? FindByEmail(string email);
    bool Create(Usuario usuario);
    bool Update(Usuario usuario);
    bool IsUniqueUser(string usuario);
    ICollection<UsuarioDto> FindAll();
  }
}
