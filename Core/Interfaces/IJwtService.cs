namespace Core.Services;

public interface IJwtService
{
    /// <summary>
    /// Genera un token JWT para el usuario especificado utilizando su identificador único.
    /// </summary>
    /// <param name="userId">El identificador único del usuario.</param>
    /// <returns>
    /// Un <see cref="string"/> que representa el token JWT generado.
    /// </returns>
    /// <exception cref="SecurityTokenException">
    /// Se lanza si ocurre un error durante la generación del token.
    /// </exception>
    /// <remarks>
    /// El token JWT generado incluirá información relevante sobre el usuario y su rol, 
    /// además de una fecha de expiración según las políticas de autenticación definidas.
    /// </remarks>
    string GenerateJwtToken(int userId);


    /// <summary>
    /// Extrae y devuelve el identificador del usuario desde un token JWT proporcionado.
    /// </summary>
    /// <param name="token">El token JWT del cual se extraerá el identificador de usuario.</param>
    /// <returns>
    /// Un <see cref="int"/> que representa el identificador del usuario contenido en el token.
    /// </returns>
    /// <exception cref="SecurityTokenException">
    /// Se lanza si el token es inválido, ha expirado o no contiene el identificador de usuario.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Se lanza si el parámetro <paramref name="token"/> es nulo o vacío.
    /// </exception>
    /// <remarks>
    /// Este método valida y decodifica el token JWT, extrayendo la información del usuario almacenada en él.
    /// </remarks>
    int GetUserIdFromToken(string token);


    /// <summary>
    /// Obtiene el token JWT desde el encabezado de autorización de la solicitud HTTP.
    /// </summary>
    /// <returns>
    /// Un <see cref="string"/> que contiene el token JWT si está presente en la solicitud, o <c>null</c> si no se encuentra.
    /// </returns>
    /// <remarks>
    /// Este método busca el token en el encabezado HTTP "Authorization" con el esquema "Bearer".
    /// Si el encabezado no está presente o no contiene un token válido, se devuelve <c>null</c>.
    /// </remarks>
    string? GetTokenFromHeader();


}