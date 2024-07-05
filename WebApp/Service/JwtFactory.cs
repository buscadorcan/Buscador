using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using WebApp.Service.IService;
using System.Text;

namespace WebApp.Service
{
    public class JwtFactory : IJwtFactory
    {
        public JwtSecurityTokenHandler CreateTokenHandler()
        {
            return new JwtSecurityTokenHandler();
        }

        public SigningCredentials CreateSigningCredentials(string secret)
        {
            var key = Encoding.ASCII.GetBytes(secret);
            return new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        }

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
