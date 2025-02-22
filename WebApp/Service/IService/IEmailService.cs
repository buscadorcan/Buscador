
namespace WebApp.Service.IService
{
    public interface IEmailService
    {
        /// <summary>
        /// Envía un correo electrónico de forma asíncrona a un destinatario específico con el asunto y el cuerpo proporcionados.
        /// </summary>
        /// <param name="destinatario">Dirección de correo electrónico del destinatario.</param>
        /// <param name="asunto">Asunto del correo electrónico.</param>
        /// <param name="cuerpo">Cuerpo del mensaje en formato HTML o texto plano.</param>
        /// <returns>
        /// Devuelve un <see cref="Task{bool}"/> que representa el resultado de la operación:
        /// <c>true</c> si el correo se envió correctamente, <c>false</c> en caso de error.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="destinatario"/>, <paramref name="asunto"/> o <paramref name="cuerpo"/> son nulos o vacíos.
        /// </exception>
        /// <exception cref="SmtpException">
        /// Se lanza si ocurre un error en el envío del correo debido a problemas en la configuración del servidor SMTP.
        /// </exception>
        Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);

    }
}