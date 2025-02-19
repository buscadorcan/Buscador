namespace WebApp.Service.IService
{
    public interface IPasswordService
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GenerateTemporaryPassword: Genera una contraseña temporal aleatoria con la longitud especificada.
         */
        string GenerateTemporaryPassword(int length);
    }
}