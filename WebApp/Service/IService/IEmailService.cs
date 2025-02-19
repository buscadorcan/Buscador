
namespace WebApp.Service.IService
{
    public interface IEmailService
    {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/EnviarCorreoAsync: Env�a un correo electr�nico de forma as�ncrona a un destinatario espec�fico con el asunto y el cuerpo proporcionados.
         */
        Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpo);
    }
}