using SharedApp.Models.Dtos;

namespace WebApp.Service.IService
{
    /// <summary>
    /// Defines a contract for authentication services to authenticate users based on their credentials.
    /// </summary>
    public interface IAuthenticateService
    {
        /// <summary>
        /// Authenticates a user based on the provided credentials.
        /// </summary>
        /// <param name="usuarioAutenticacionDto">The user authentication data transfer object containing the username and password.</param>
        /// <returns>
        /// A <see cref="UsuarioAutenticacionRespuestaDto"/> containing the authentication result.
        /// If the authentication is successful, returns a response with the user's authentication details (e.g., token, user information).
        /// If the authentication fails, returns a response indicating the failure (e.g., invalid credentials).
        /// In case of an exception, returns an appropriate error response.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown if any error occurs during the authentication process, such as invalid credentials, missing user data, or database errors.
        /// </exception>
        Result<UsuarioAutenticacionRespuestaDto> Authenticate(UsuarioAutenticacionDto usuarioAutenticacionDto);
    }
}
