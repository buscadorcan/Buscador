using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Core.Services
{
    public interface IJwtFactory
    {
        /// <summary>
        /// Crea y devuelve una instancia de <see cref="JwtSecurityTokenHandler"/> para la generación y validación de tokens JWT.
        /// </summary>
        /// <returns>
        /// Una instancia de <see cref="JwtSecurityTokenHandler"/> que permite crear y validar tokens de autenticación.
        /// </returns>
        /// <remarks>
        /// Este método facilita la creación de un manejador de tokens JWT, el cual es esencial para gestionar la autenticación basada en tokens.
        /// </remarks>
        JwtSecurityTokenHandler CreateTokenHandler();


        /// <summary>
        /// Genera las credenciales de firma necesarias para la creación de un token JWT utilizando una clave secreta.
        /// </summary>
        /// <param name="secret">La clave secreta utilizada para firmar el token.</param>
        /// <returns>
        /// Una instancia de <see cref="SigningCredentials"/> que contiene la clave de firma y el algoritmo utilizado.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si el parámetro <paramref name="secret"/> es nulo o está vacío.
        /// </exception>
        /// <remarks>
        /// Este método convierte la clave secreta en una clave de seguridad válida para la firma de tokens JWT.
        /// Se recomienda utilizar una clave segura y almacenarla en un entorno seguro.
        /// </remarks>
        SigningCredentials CreateSigningCredentials(string secret);


        /// <summary>
        /// Configura los parámetros de validación para la verificación de tokens JWT utilizando una clave secreta.
        /// </summary>
        /// <param name="secret">La clave secreta utilizada para validar la firma del token.</param>
        /// <returns>
        /// Una instancia de <see cref="TokenValidationParameters"/> con los parámetros de validación configurados.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si el parámetro <paramref name="secret"/> es nulo o está vacío.
        /// </exception>
        /// <remarks>
        /// Este método configura los parámetros que se utilizarán para validar tokens JWT, incluyendo la clave de firma,
        /// la validación de audiencia/emisor y la verificación de la fecha de expiración.
        /// </remarks>
        TokenValidationParameters CreateTokenValidationParameters(string secret);


    }
}