using System.Security.Claims;
using System.Text.Json;

namespace SharedApp.Helpers
{
    public class JwtParser
    {
        /// <summary>
        /// Clase .cs que se usa para formatear margenes y no haya problemas al momento de ejecutar las sentencias del buscador
        /// </summary>
        /// <param name="jwt"></param>
        /// <returns></returns>
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var jsonBytes = ParsearEnBase64SinMargen(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }

        private static byte[] ParsearEnBase64SinMargen(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
    
}
