namespace Core.Services
{
    /// <summary>
    /// Define un servicio para el envío de correos electrónicos.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico.
        /// </summary>
        /// <param name="to">Dirección de correo electrónico del destinatario.</param>
        /// <param name="subject">Asunto del correo electrónico.</param>
        /// <param name="body">Cuerpo del correo electrónico.</param>
        /// <returns>Devuelve <see langword="true"/> si el correo electrónico se envió correctamente; de lo contrario, <see langword="false"/>.</returns>
        Task<bool> SendEmailAsync(string to, string subject, string body);

    }
}