using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebApp.Service.IService;
using System.Text;

namespace WebApp.Service
{
  public class JwtFactory : IJwtFactory
  {
    /// <summary>
    /// Crea una instancia de <see cref="JwtSecurityTokenHandler"/>.
    /// </summary>
    /// <returns>Una nueva instancia de <see cref="JwtSecurityTokenHandler"/>.</returns>
    public JwtSecurityTokenHandler CreateTokenHandler()
    {
      return new JwtSecurityTokenHandler();
    }

    /// <summary>
    /// Crea las credenciales de firma necesarias para firmar un token JWT.
    /// </summary>
    /// <param name="secret">El secreto compartido que se utilizará para la firma del token.</param>
    /// <returns>Una instancia de <see cref="SigningCredentials"/> para firmar el token JWT.</returns>
    public SigningCredentials CreateSigningCredentials(string secret)
    {
      var key = Encoding.ASCII.GetBytes(secret);
      return new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
    }

    /// <summary>
    /// Crea los parámetros de validación del token JWT, que se utilizan para verificar su autenticidad.
    /// </summary>
    /// <param name="secret">El secreto compartido que se utilizará para validar la firma del token.</param>
    /// <returns>Una instancia de <see cref="TokenValidationParameters"/> para la validación del token JWT.</returns>
    public TokenValidationParameters CreateTokenValidationParameters(string secret)
    {
      var key = Encoding.ASCII.GetBytes(secret);
      return new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
      };
    }
  }
}
