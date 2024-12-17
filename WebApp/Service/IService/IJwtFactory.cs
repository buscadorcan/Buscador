using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Service.IService
{
  /// <summary>
  /// Define el contrato para la creación y manejo de JSON Web Tokens (JWT) en el sistema.
  /// </summary>
  public interface IJwtFactory
  {
    /// <summary>
    /// Crea un manejador de tokens JWT.
    /// </summary>
    /// <returns>Una instancia de <see cref="JwtSecurityTokenHandler"/> que puede ser utilizada para crear y validar JWT.</returns>
    JwtSecurityTokenHandler CreateTokenHandler();

    /// <summary>
    /// Crea las credenciales de firma utilizadas para firmar el token JWT.
    /// </summary>
    /// <param name="secret">Una cadena que contiene la clave secreta utilizada para firmar el token.</param>
    /// <returns>Las credenciales de firma necesarias para firmar el JWT.</returns>
    SigningCredentials CreateSigningCredentials(string secret);

    /// <summary>
    /// Crea los parámetros de validación necesarios para validar un JWT.
    /// </summary>
    /// <param name="secret">Una cadena que contiene la clave secreta utilizada para validar el token.</param>
    /// <returns>Los parámetros de validación utilizados para verificar la autenticidad del JWT.</returns>
    TokenValidationParameters CreateTokenValidationParameters(string secret);
  }
}
