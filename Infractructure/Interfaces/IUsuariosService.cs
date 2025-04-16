using Infractruture.Models;
using SharedApp.Dtos;
using SharedApp.Response;

namespace Infractruture.Interfaces
{
    /// <summary>
    /// Interfaz que define los servicios relacionados con la gestión de usuarios en la aplicación.
    /// </summary>
    public interface IUsuariosService
    {
        /// <summary>
        /// Obtiene la lista de todos los usuarios.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="UsuarioDto"/> representando los usuarios.</returns>
        Task<List<UsuarioDto>> GetUsuariosAsync();

        /// <summary>
        /// Obtiene la lista de roles disponibles en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="VwRolDto"/> representando los roles.</returns>
        Task<List<VwRolDto>> GetRolesAsync();

        /// <summary>
        /// Obtiene la lista de ONA disponibles en el sistema.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="OnaDto"/> representando los ONA.</returns>
        Task<List<OnaDto>> GetOnaAsync();

        /// <summary>
        /// Obtiene los detalles de un usuario específico por su ID.
        /// </summary>
        /// <param name="IdUsuario">El ID del usuario a obtener.</param>
        /// <returns>Un objeto <see cref="UsuarioDto"/> que representa al usuario con el ID especificado.</returns>
        Task<UsuarioDto> GetUsuarioAsync(int IdUsuario);

        /// <summary>
        /// Registra o actualiza un usuario en el sistema.
        /// </summary>
        /// <param name="usuarioParaRegistro">El objeto <see cref="UsuarioDto"/> que contiene los datos del usuario a registrar o actualizar.</param>
        /// <returns>Un objeto <see cref="RespuestaRegistro"/> que indica el resultado de la operación.</returns>
        Task<RespuestaRegistro> RegistrarOActualizar(UsuarioDto usuarioParaRegistro);

        /// <summary>
        /// Elimina un usuario del sistema.
        /// </summary>
        /// <param name="IdUsuario">El ID del usuario a eliminar.</param>
        /// <returns>Un valor booleano que indica si la eliminación fue exitosa.</returns>
        Task<bool> DeleteUsuarioAsync(int IdUsuario);

        /// <summary>
        /// Valida si el correo electrónico proporcionado es único en el sistema.
        /// </summary>
        /// <param name="email">El correo electrónico a validar.</param>
        /// <returns>Un valor booleano que indica si el correo electrónico es único.</returns>
        Task<bool> ValidarEmailUnico(string email);

        /// <summary>
        /// Actualiza la contraseña de un usuario.
        /// </summary>
        /// <param name="usuarioCambiarClave">El objeto <see cref="UsuarioCambiarClaveDto"/> que contiene los datos de la contraseña a actualizar.</param>
        /// <returns>Un objeto <see cref="RespuestasAPI{T}"/> que indica el resultado de la operación, con un valor booleano que representa si la actualización fue exitosa.</returns>
        Task<RespuestasAPI<bool>> ActualizarClave(UsuarioCambiarClaveDto usuarioCambiarClave);
    }
}
