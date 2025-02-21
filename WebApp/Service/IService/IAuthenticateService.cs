using SharedApp.Models.Dtos;

namespace WebApp.Service.IService
{
    /// <summary>
    /// Defines a contract for authentication services to authenticate users based on their credentials.
    /// </summary>
    public interface IAuthenticateService
    {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Authenticate: Autentica a un usuario basado en las credenciales proporcionadas.
         * Devuelve un objeto con los detalles de autenticaci�n del usuario si es exitoso,
         * o un mensaje de error si las credenciales son inv�lidas.
         */
        Result<AuthenticateResponseDto> Authenticate(UsuarioAutenticacionDto usuarioAutenticacionDto);
        Result<UsuarioAutenticacionRespuestaDto> ValidateCode(AuthValidationDto authValidationDto);
    }
}
