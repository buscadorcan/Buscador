using Google.Apis.Gmail.v1;

namespace Core.Services
{
  /// <summary>
  /// Implementación de <see cref="IGmailClientFactory"/> que crea instancias de <see cref="GmailService"/> configuradas para el envío de correos electrónicos con OAuth 2.0.
  /// </summary>
  public interface IGmailClientFactory
  {
    /// <summary>
    /// Crea una instancia de <see cref="GmailService"/> configurada para el envío de correos electrónicos con OAuth 2.0.
    /// </summary>
    /// <returns>Instancia de <see cref="GmailService"/> configurada para el envío de correos electrónicos con OAuth 2.0.</returns>
      Task<GmailService> CreateGmailServiceAsync();
  }
}
