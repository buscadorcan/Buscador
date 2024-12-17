namespace WebApp.Service.IService
{
  /// <summary>
  /// Define un contrato para el servicio de envío de correos electrónicos.
  /// </summary>
  public interface IEmailService
  {
    /// <summary>
    /// Envía un correo electrónico de manera asíncrona.
    /// </summary>
    /// <param name="destinatario">La dirección de correo electrónico del destinatario.</param>
    /// <param name="asunto">El asunto del correo electrónico.</param>
    /// <param name="cuerpo">El cuerpo del correo electrónico, que puede incluir texto o HTML.</param>
    /// <returns>Una tarea que representa la operación asincrónica de envío del correo electrónico.</returns>
    Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);
  }
}
