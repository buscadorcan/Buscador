using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Service.IService
{
    public interface IJwtFactory
    {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateTokenHandler: Crea un manejador de tokens JWT para la generaci�n y validaci�n de tokens de autenticaci�n.
         */
        JwtSecurityTokenHandler CreateTokenHandler();

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateSigningCredentials: Genera las credenciales de firma necesarias para la creaci�n de un token JWT utilizando una clave secreta.
         */
        SigningCredentials CreateSigningCredentials(string secret);

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateTokenValidationParameters: Configura los par�metros de validaci�n para la verificaci�n de tokens JWT utilizando una clave secreta.
         */
        TokenValidationParameters CreateTokenValidationParameters(string secret);

    }
}