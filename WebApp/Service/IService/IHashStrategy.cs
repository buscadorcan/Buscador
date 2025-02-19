namespace WebApp.Service.IService
{
    public interface IHashStrategy
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ComputeHash: Calcula un hash seguro basado en la cadena de entrada proporcionada.
         */
        string ComputeHash(string? input);
    }
}
