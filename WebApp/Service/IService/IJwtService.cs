namespace WebApp.Service.IService
{
    public interface IJwtService
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GenerateJwtToken: Genera un token JWT para el usuario especificado con su identificador único.
         */
        string GenerateJwtToken(int userId);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetUserIdFromToken: Extrae y devuelve el identificador de usuario a partir de un token JWT.
         */
        int GetUserIdFromToken(string token);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetTokenFromHeader: Obtiene el token JWT desde el encabezado de la solicitud HTTP.
         */
        string? GetTokenFromHeader();

    }
}