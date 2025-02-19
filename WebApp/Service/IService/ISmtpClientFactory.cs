using System.Net.Mail;

namespace WebApp.Service.IService
{
  public interface ISmtpClientFactory
  {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateSmtpClient: Crea y devuelve una nueva instancia de SmtpClient para el envío de correos electrónicos.
         */
        SmtpClient CreateSmtpClient();
  }
}
