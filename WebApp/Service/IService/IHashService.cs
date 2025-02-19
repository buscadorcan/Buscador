namespace WebApp.Service.IService
{
    public interface IHashService
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GenerateHash: Genera un hash único basado en la cadena de entrada proporcionada.
         */
        string GenerateHash(string? input);
    }
}
