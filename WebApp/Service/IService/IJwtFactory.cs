using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace WebApp.Service.IService
{
    public interface IJwtFactory
    {
        JwtSecurityTokenHandler CreateTokenHandler();
        SigningCredentials CreateSigningCredentials(string secret);
        TokenValidationParameters CreateTokenValidationParameters(string secret);
    }
}