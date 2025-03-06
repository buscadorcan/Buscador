
using SharedApp.Models.Dtos;

namespace WebApp.Service.IService
{
    /// <summary>
    /// Define un servicio para el env铆o de correos electr贸nicos.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Env铆a un correo electr贸nico.
        /// </summary>
        /// <param name="to">Direcci贸n de correo electr贸nico del destinatario.</param>
        /// <param name="subject">Asunto del correo electr贸nico.</param>
        /// <param name="body">Cuerpo del correo electr贸nico.</param>
        /// <returns>Devuelve <see langword="true"/> si el correo electr贸nico se envi贸 correctamente; de lo contrario, <see langword="false"/>.</returns>
        Task<bool> SendEmailAsync(string to, string subject, string body);


        /// <summary>
        /// Env韆 un correo electr髇ico de forma as韓crona seg鷑 una opci髇 respectiva
        /// </summary>
        Task<bool> EnviarCorreoAlerta(EmailDto email);
    }
}