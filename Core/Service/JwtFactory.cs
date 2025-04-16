using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Core.Services
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
