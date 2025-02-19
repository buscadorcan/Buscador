
namespace WebApp.Service.IService
{
    public interface IEmailService
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/EnviarCorreoAsync: Envía un correo electrónico de forma asíncrona a un destinatario específico con el asunto y el cuerpo proporcionados.
         */
        Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);
    }
}