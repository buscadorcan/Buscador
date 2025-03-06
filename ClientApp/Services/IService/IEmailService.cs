using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService
{
    public interface IEmailService
    {
        Task<bool> Enviar(EmailDto email, string endpoint);
    }
}
