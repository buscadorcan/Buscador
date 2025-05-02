using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controlador para la autenticación con Google.
    /// </summary>
    [Route(Routes.GOOGLE)]
    [ApiController]
    public class GoogleAuthController : ControllerBase
    {
        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<GoogleAuthController> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="GoogleAuthController"/>.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación.</param>
        /// <param name="logger">Logger.</param>
        public GoogleAuthController(IConfiguration configuration, ILogger<GoogleAuthController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Endpoint para redirigir al usuario a Google para la autenticación.
        /// </summary>
        [HttpGet(Routes.AUTHORIZE)]
        public IActionResult Authorize()
        {
            // Verificar si la carpeta existe
            if (Directory.Exists(_configuration["GoogleOAuth:Tokens"]))
            {
                // Si la carpeta existe, obtenemos los archivos dentro de ella
                var tokenFiles = Directory.GetFiles(_configuration["GoogleOAuth:Tokens"]);

                if (tokenFiles.Length > 0)
                {
                    _logger.LogInformation("Token existente. No es necesario autenticarse.");
                    return Ok(new { message = "Ya existe un token válido. No es necesario autenticarse nuevamente." });
                }
            }
            else
            {
                // Si la carpeta no existe, creamos la carpeta
                Directory.CreateDirectory(_configuration["GoogleOAuth:Tokens"]);
            }

            try
            {
                // Cargar las credenciales desde la configuración de appsettings.json
                var clientId = _configuration["GoogleOAuth:ClientId"];
                var clientSecret = _configuration["GoogleOAuth:ClientSecret"];
                var redirectUri = _configuration["GoogleOAuth:RedirectUri"];

                var clientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };

                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets,
                    Scopes = new[] { "https://www.googleapis.com/auth/gmail.send" },
                    DataStore = new FileDataStore(_configuration["GoogleOAuth:Tokens"], true)
                });

                // Generar la URL de autorización
                string authorizationUrl = flow.CreateAuthorizationCodeRequest(redirectUri).Build().ToString();

                _logger.LogInformation("Redirigiendo a Google para autenticación.");
                return Redirect(authorizationUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la URL de autorización: {ex.Message}");
                return BadRequest("Hubo un problema al generar la URL de autorización.");
            }
        }

        /// <summary>
        /// Endpoint de callback para manejar la respuesta de Google después de la autenticación.
        /// </summary>
        [HttpGet(Routes.CALLBACK)]
        public async Task<IActionResult> Callback(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Error: Código de autorización no recibido.");
            }

            try
            {
                // Cargar las credenciales desde la configuración de appsettings.json
                var clientId = _configuration["GoogleOAuth:ClientId"];
                var clientSecret = _configuration["GoogleOAuth:ClientSecret"];
                var redirectUri = _configuration["GoogleOAuth:RedirectUri"];
                var userEmail = _configuration["GoogleOAuth:Username"];

                var clientSecrets = new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                };

                var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets,
                    Scopes = new[] { "https://www.googleapis.com/auth/gmail.compose" },
                    DataStore = new FileDataStore(_configuration["GoogleOAuth:Tokens"], true)
                });

                // Intercambiar el código de autorización por el token de acceso
                TokenResponse token = await flow.ExchangeCodeForTokenAsync(
                    userEmail, code, redirectUri, CancellationToken.None);

                // Mostrar el token en la consola o en el log
                _logger.LogInformation("Token obtenido: {Token}", JsonConvert.SerializeObject(token));

                _logger.LogInformation("Autenticación exitosa. Token guardado.");
                return Ok(new { message = "Autenticación exitosa. Token guardado." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en la autenticación: {ex.Message}");
                return BadRequest($"Error en la autenticación: {ex.Message}");
            }
        }
    }
}
