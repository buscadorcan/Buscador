using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IUsuarioEmailRepository
    {
        
        /// <summary>
        ///Obtiene un usuario de la base de datos según el nombre
        /// </summary>
        UserEmailDto ObtenerUsuario(string User);

        /// <summary>
        /// Obtiene la lista de usuario de una determinada ONA
        /// </summary>
        List<UserEmailDto> ObtenerUsuarios(int IdOna);
    }
}
