using System.Net.Mail;
using WebApp.Service.IService;

namespace WebApp.Service
{
    public class EmailService(
        ISmtpClientFactory smtpClientFactory,
        IConfiguration configuration,
        ILogger<EmailService> logger
    ) : IEmailService
    {
        private readonly ISmtpClientFactory _smtpClientFactory = smtpClientFactory;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<EmailService> _logger = logger;
        public async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo)
        {
            var smtpClient = _smtpClientFactory.CreateSmtpClient();

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["EmailSettings:From"] ?? ""),
                Subject = asunto,
                Body = cuerpo,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(destinatario);

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Correo enviado a {0}", destinatario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando correo a {0}", destinatario);
            }
        }
    }
}