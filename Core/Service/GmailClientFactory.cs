using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Gmail.v1;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Core.Services
{
    /// <summary>
    /// Implementación de <see cref="IGmailClientFactory"/> que crea instancias de <see cref="GmailService"/> configuradas para el envío de correos electrónicos con OAuth 2.0.
    /// </summary>
    public class GmailClientFactory : Services.IGmailClientFactory
    {
        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="GmailClientFactory"/>.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación.</param>
        public GmailClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public async Task<GmailService> CreateGmailServiceAsync()
        {
            var accessToken = await GetAccessTokenAsync();

            // Configurar el servicio Gmail con el token de acceso
            var credential = GoogleCredential.FromAccessToken(accessToken);

            return new GmailService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = _configuration["GoogleOAuth:AppName"]
            });
        }

        /// <summary>
        /// Obtiene un token de acceso válido.
        /// </summary>
        /// <returns>Token de acceso.</returns>
        private async Task<string> GetAccessTokenAsync()
        {
            if (!Directory.Exists(_configuration["GoogleOAuth:Tokens"]))
            {
                throw new InvalidOperationException("El directorio del token no existe. Autenticación requerida.");
            }

            var tokenFiles = Directory.GetFiles(_configuration["GoogleOAuth:Tokens"]);
            if (tokenFiles.Length == 0)
            {
                throw new InvalidOperationException("No se encontraron archivos de token. Autenticación requerida.");
            }

            var tokenJson = await File.ReadAllTextAsync(tokenFiles[0]);
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(tokenJson);

            if (tokenResponse.IssuedUtc.AddSeconds(tokenResponse.ExpiresInSeconds ?? 0) < DateTime.UtcNow)
            {
                tokenResponse = await RefreshTokenAsync(tokenResponse.RefreshToken);
                await File.WriteAllTextAsync(tokenFiles[0], JsonConvert.SerializeObject(tokenResponse));
            }

            return tokenResponse.AccessToken;
        }

        /// <summary>
        /// Refresca un token de acceso.
        /// </summary>
        /// <param name="refreshToken">Token de actualización.</param>
        /// <returns>Token de respuesta.</returns>
        private async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var clientSecrets = new ClientSecrets
            {
                ClientId = _configuration["GoogleOAuth:ClientId"],
                ClientSecret = _configuration["GoogleOAuth:ClientSecret"]
            };

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = clientSecrets,
                Scopes = new[] { "https://www.googleapis.com/auth/gmail.compose" }
            });

            return await flow.RefreshTokenAsync(_configuration["GoogleOAuth:Username"], refreshToken, CancellationToken.None);
        }
    }
}
