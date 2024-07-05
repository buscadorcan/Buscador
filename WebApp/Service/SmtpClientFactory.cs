using System.Net;
using System.Net.Mail;
using WebApp.Service.IService;

namespace WebApp.Service
{
    public class SmtpClientFactory(IConfiguration configuration) : ISmtpClientFactory
    {
        private readonly IConfiguration _configuration = configuration;
        public SmtpClient CreateSmtpClient()
        {
            return new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"] ?? "0"),
                Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"]),
                EnableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true")
            };
        }
    }
}