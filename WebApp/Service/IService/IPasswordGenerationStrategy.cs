namespace WebApp.Service.IService
{
    public interface IPasswordGenerationStrategy
    {
        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GeneratePassword: Genera una contrase�a aleatoria con la longitud especificada.
         */
        string GeneratePassword(int length);
    }
}