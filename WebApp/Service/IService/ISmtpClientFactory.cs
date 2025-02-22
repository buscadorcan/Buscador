using System.Net.Mail;

namespace WebApp.Service.IService
{
  public interface ISmtpClientFactory
  {
        /// <summary>
        /// Crea y devuelve una nueva instancia de <see cref="SmtpClient"/> configurada 
        /// para el envío de correos electrónicos.
        /// </summary>
        /// <returns>Devuelve una instancia de <see cref="SmtpClient"/> lista para ser utilizada.</returns>
        SmtpClient CreateSmtpClient();
  }
}
