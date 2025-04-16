using SharedApp.Dtos;

namespace Infractruture.Interfaces
{
    public interface IEmailService
    {
        Task<bool> Enviar(EmailDto email, string endpoint);
    }
}
