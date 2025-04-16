using SharedApp.Dtos;
using SharedApp.Response;

namespace Infractruture.Interfaces
{
    /// <summary>
    /// Define los métodos de autenticación que deben ser implementados por un servicio de autenticación.
    /// </summary>
    public interface IServiceAutenticacion
    {
        /// <summary>
        /// Autentica a un usuario con las credenciales proporcionadas.
        /// </summary>
        /// <param name="usuarioAutenticacionDto">Objeto que contiene el nombre de usuario y la contraseña.</param>
        /// <returns>Una respuesta de API que contiene los datos de autenticación si la autenticación es exitosa.</returns>
        Task<RespuestasAPI<AuthenticateResponseDto>> Autenticar(UsuarioAutenticacionDto usuarioAutenticacionDto);

        /// <summary>
        /// Valida un código de acceso para completar el proceso de autenticación.
        /// </summary>
        /// <param name="authValidationDto">Objeto que contiene los datos necesarios para la validación.</param>
        /// <returns>Una respuesta de API con los datos del usuario autenticado si la validación es exitosa.</returns>
        Task<RespuestasAPI<UsuarioAutenticacionRespuestaDto>> Acceder(AuthValidationDto authValidationDto);

        /// <summary>
        /// Recupera la información de un usuario con base en los datos proporcionados.
        /// </summary>
        /// <typeparam name="T">Tipo de dato esperado en la respuesta.</typeparam>
        /// <param name="usuarioRecuperacionDto">Objeto que contiene la información del usuario a recuperar.</param>
        /// <returns>Una respuesta de API con la información del usuario, si existe.</returns>
        Task<RespuestasAPI<T>?> Recuperar<T>(UsuarioRecuperacionDto usuarioRecuperacionDto);

        /// <summary>
        /// Cierra la sesión del usuario autenticado.
        /// </summary>
        /// <returns>Una tarea completada cuando la sesión se haya cerrado.</returns>
        Task Salir();
    }
}
