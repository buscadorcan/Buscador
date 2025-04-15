using SharedApp.Dtos;

namespace Core.Interfaces
{
    public interface IAuthenticateService
    {
        /// <summary>
        /// Autentica a un usuario basado en las credenciales proporcionadas.
        /// </summary>
        /// <param name="usuarioAutenticacionDto">Objeto <see cref="UsuarioAutenticacionDto"/> que contiene el nombre de usuario y contraseña.</param>
        /// <returns>
        /// Devuelve un objeto <see cref="Task{TResult}"/> donde T es <see cref="Result{AuthenticateResponseDto}"/>.
        Result<AuthenticateResponseDto> Authenticate(UsuarioAutenticacionDto usuarioAutenticacionDto);
        /// En caso contrario, devuelve un mensaje de error.
        /// </returns>

        /// <summary>
        /// Valida un código de autenticación recibido tras el inicio de sesión.
        /// </summary>
        /// <param name="authValidationDto">Objeto <see cref="AuthValidationDto"/> que contiene el código de validación.</param>
        /// <returns>
        /// Devuelve un objeto <see cref="Result{UsuarioAutenticacionRespuestaDto}"/>.
        /// Si la validación es exitosa, el resultado contiene los datos del usuario autenticado.
        /// En caso contrario, devuelve un mensaje de error.
        /// </returns>
        Result<UsuarioAutenticacionRespuestaDto> ValidateCode(AuthValidationDto authValidationDto);

    }
}
