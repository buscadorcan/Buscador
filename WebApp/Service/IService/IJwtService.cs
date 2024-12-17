namespace WebApp.Service.IService
{
  /// <summary>
  /// Define los métodos necesarios para la gestión de JSON Web Tokens (JWT) en el sistema.
  /// </summary>
  public interface IJwtService
  {
    /// <summary>
    /// Genera un token JWT para un usuario basado en su ID.
    /// </summary>
    /// <param name="userId">El ID del usuario para quien se generará el token.</param>
    /// <returns>Un token JWT como cadena que puede ser utilizado para la autenticación.</returns>
    string GenerateJwtToken(int userId);

    /// <summary>
    /// Extrae el ID del usuario de un token JWT.
    /// </summary>
    /// <param name="token">El token JWT del cual se extraerá el ID del usuario.</param>
    /// <returns>El ID del usuario extraído del token JWT.</returns>
    int GetUserIdFromToken(string token);

    /// <summary>
    /// Obtiene el token JWT de los encabezados de la solicitud HTTP.
    /// </summary>
    /// <returns>El token JWT encontrado en el encabezado de la solicitud, o null si no se encuentra.</returns>
    string? GetTokenFromHeader();
  }
}
