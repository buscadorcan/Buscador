using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using MimeKit;
using WebApp.Service.IService;

namespace WebApp.Service
{
    /// <summary>
    /// Implementación de <see cref="IEmailService"/> que envía correos electrónicos a través de Gmail API.
    /// </summary>
    public class EmailService : IEmailService
    {
        /// <summary>
        /// Factoría para crear instancias de <see cref="GmailService"/>.
        /// </summary>
        private readonly IGmailClientFactory _gmailClientFactory;

        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<EmailService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="EmailService"/>.
        /// </summary>
        /// <param name="gmailClientFactory">Factoría para crear instancias de <see cref="GmailService"/>.</param>
        /// <param name="configuration">Configuración de la aplicación.</param>
        /// <param name="logger">Logger.</param>
        public EmailService(IGmailClientFactory gmailClientFactory, IConfiguration configuration, ILogger<EmailService> logger)
        {
            _gmailClientFactory = gmailClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // Crear servicio de Gmail usando la factoría
                var gmailService = await _gmailClientFactory.CreateGmailServiceAsync();

                // Construir el mensaje MIME
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_configuration["GoogleOAuth:AppName"], _configuration["GoogleOAuth:Username"] ?? throw new InvalidOperationException("Username is required")));
                message.To.Add(new MailboxAddress(to, to));
                message.Subject = subject;
                var textPart = new TextPart("html")
                {
                    Text = body,
                    ContentTransferEncoding = ContentEncoding.Base64  // Asegurando que el mensaje se codifique correctamente en base64
                };

                message.Body = textPart;

                // Convertir MIME a base64 para enviar por Gmail API
                var rawMessage = Base64UrlEncode(message.ToString());

                // Enviar el mensaje usando Gmail API
                var gmailMessage = new Message
                {
                    Raw = rawMessage
                };

                await gmailService.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
                return true; // Éxito
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo a {Email}", to);
                return false; // Fallo
            }
        }

        /// <summary>
        /// Codifica un mensaje MIME en base64 para enviar por Gmail API.
        /// </summary>
        /// <param name="input">Mensaje MIME.</param>
        /// <returns>Mensaje MIME codificado en base64.</returns>
        private static string Base64UrlEncode(string input)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(input);
            var base64 = Convert.ToBase64String(bytes);
            return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }
    
        public async Task<bool> EnviarCorreoAlerta(EmailDto email)
        {
            string html = "";
            try
            {
                if (email.Opcion == 1) { html = _configuration["EmailTemplates:create_user"] ?? ""; }
                else if (email.Opcion == 2) { html = _configuration["EmailTemplates:migration"] ?? ""; }
                else if (email.Opcion == 3) { html = _configuration["EmailTemplates:create_update_esquema"] ?? ""; }
                else if (email.Opcion == 4) { html = _configuration["EmailTemplates:update_columns_esquema"] ?? ""; }

                if (File.Exists(html))
                {
                    string htmlBody = File.ReadAllText(html);
                    var user = _usuarioRepository.ObtenerUsuario(email.usuario);
                    string Destinatarios = user.Email + ",";
                    if (user.CodigoHomologacion.Contains("KEY_USER_ONA"))
                    {
                        var usersOna = _usuarioRepository.ObtenerUsuarios(user.IdOna);
                        if (usersOna.Count > 0)
                        {
                            foreach (var usuario in usersOna)
                            {
                                Destinatarios += usuario.Email + ",";
                            }
                        }

                    }
                    htmlBody = string.Format(htmlBody, email.usuario);
                    return await SendEmailAsync(Destinatarios ?? "", "Creación de Usuario", htmlBody);
                }
                else
                {
                    throw new FileNotFoundException("La plantilla de correo no se encuentra en la ubicaci�n especificada.");
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }
    }
}
