
namespace WebApp.Service.IService
{
    public interface IEmailService
    {
        /// <summary>
        /// Env�a un correo electr�nico de forma as�ncrona a un destinatario espec�fico con el asunto y el cuerpo proporcionados.
        /// </summary>
        /// <param name="destinatario">Direcci�n de correo electr�nico del destinatario.</param>
        /// <param name="asunto">Asunto del correo electr�nico.</param>
        /// <param name="cuerpo">Cuerpo del mensaje en formato HTML o texto plano.</param>
        /// <returns>
        /// Devuelve un <see cref="Task{bool}"/> que representa el resultado de la operaci�n:
        /// <c>true</c> si el correo se envi� correctamente, <c>false</c> en caso de error.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="destinatario"/>, <paramref name="asunto"/> o <paramref name="cuerpo"/> son nulos o vac�os.
        /// </exception>
        /// <exception cref="SmtpException">
        /// Se lanza si ocurre un error en el env�o del correo debido a problemas en la configuraci�n del servidor SMTP.
        /// </exception>
        Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);

    }
}