using System.Net.Mail;

namespace WebApp.Service.IService
{
    public interface ISmtpClientFactory
    {
        SmtpClient CreateSmtpClient();
    }
}