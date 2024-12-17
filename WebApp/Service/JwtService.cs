using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using WebApp.Service.IService;

namespace WebApp.Service
{
  public class JwtService(
    IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor,
    IJwtFactory jwtFactory
  ) : IJwtService
  {
    private readonly IConfiguration _configuration = configuration;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IJwtFactory _jwtFactory = jwtFactory;

    /// <summary>
    /// Genera un token JWT para un usuario basado en su ID.
    /// </summary>
    /// <param name="userId">El ID del usuario para quien se generará el token.</param>
    /// <returns>El token JWT generado.</returns>
    public string GenerateJwtToken(int userId)
    {
      var secret = _configuration.GetValue<string>("ApiSettings:Secreta");
      var tokenHandler = _jwtFactory.CreateTokenHandler();
      var signingCredentials = _jwtFactory.CreateSigningCredentials(secret ?? "");

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new Claim[]
        {
          new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        }),
        Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials = signingCredentials
      };

      var token = tokenHandler.CreateToken(tokenDescriptor);
      return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Obtiene el ID del usuario a partir de un token JWT.
    /// </summary>
    /// <param name="token">El token JWT desde el cual se extraerá el ID del usuario.</param>
    /// <returns>El ID del usuario si es válido; de lo contrario, 0.</returns>
    public int GetUserIdFromToken(string token)
    {
      var secret = _configuration.GetValue<string>("ApiSettings:Secreta");
      var tokenHandler = _jwtFactory.CreateTokenHandler();
      var validationParameters = _jwtFactory.CreateTokenValidationParameters(secret ?? "");

      try
      {
        SecurityToken securityToken;
        var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

        var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
          return userId;
        }

        return 0;
      }
      catch (Exception)
      {
        return 0;
      }
    }

    /// <summary>
    /// Obtiene el token JWT del encabezado de autorización de la solicitud HTTP actual.
    /// </summary>
    /// <returns>El token JWT extraído del encabezado de autorización si está presente; de lo contrario, null.</returns>
    public string? GetTokenFromHeader()
    {
      var authorizationHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();
      if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
      {
        return authorizationHeader.Substring("Bearer ".Length).Trim();
      }
      return null;
    }
  }
}
