using System.Net.Mail;

namespace WebApp.Service.IService
{
  /// <summary>
  /// Define un contrato para la creación de instancias de <see cref="SmtpClient"/>.
  /// </summary>
  public interface ISmtpClientFactory
  {
    /// <summary>
    /// Crea una nueva instancia de <see cref="SmtpClient"/>.
    /// </summary>
    /// <returns>Una nueva instancia de <see cref="SmtpClient"/>.</returns>
    SmtpClient CreateSmtpClient();
  }
}
