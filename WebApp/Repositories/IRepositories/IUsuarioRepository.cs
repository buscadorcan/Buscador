using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  /// <summary>
  /// Interfaz para el repositorio de usuarios, que define las operaciones relacionadas con la gestión de usuarios.
  /// </summary>
  public interface IUsuarioRepository
  {
    /// <summary>
    /// Busca un usuario por su ID.
    /// </summary>
    /// <param name="idUsuario">ID del usuario a buscar.</param>
    /// <returns>El usuario encontrado, o <c>null</c> si no existe.</returns>
    Usuario? FindById(int idUsuario);

    /// <summary>
    /// Crea un nuevo usuario en el sistema.
    /// </summary>
    /// <param name="usuario">Entidad de usuario con los datos a guardar.</param>
    /// <returns><c>true</c> si la operación fue exitosa; de lo contrario, <c>false</c>.</returns>
    bool Create(Usuario usuario);

    /// <summary>
    /// Actualiza los datos de un usuario existente.
    /// </summary>
    /// <param name="usuario">Entidad de usuario con los datos actualizados.</param>
    /// <returns><c>true</c> si la operación fue exitosa; de lo contrario, <c>false</c>.</returns>
    bool Update(Usuario usuario);

    /// <summary>
    /// Verifica si un nombre de usuario es único en el sistema.
    /// </summary>
    /// <param name="usuario">Nombre de usuario a verificar.</param>
    /// <returns><c>true</c> si el nombre de usuario es único; de lo contrario, <c>false</c>.</returns>
    bool IsUniqueUser(string usuario);

    /// <summary>
    /// Obtiene una colección de todos los usuarios registrados.
    /// </summary>
    /// <returns>Una colección de objetos <see cref="Usuario"/>.</returns>
    ICollection<Usuario> FindAll();

    /// <summary>
    /// Autentica a un usuario en el sistema.
    /// </summary>
    /// <param name="usuarioAutenticacionDto">DTO con las credenciales de autenticación del usuario.</param>
    /// <returns>Un objeto <see cref="UsuarioAutenticacionRespuestaDto"/> con el resultado de la autenticación.</returns>
    UsuarioAutenticacionRespuestaDto Login(UsuarioAutenticacionDto usuarioAutenticacionDto);

    /// <summary>
    /// Envía instrucciones para recuperar la contraseña de un usuario.
    /// </summary>
    /// <param name="usuarioRecuperacionDto">DTO con el email del usuario que desea recuperar su contraseña.</param>
    /// <returns>Un valor booleano indicando si la operación fue exitosa.</returns>
    Task<bool> RecoverAsync(UsuarioRecuperacionDto usuarioRecuperacionDto);
  }
}
