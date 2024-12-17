namespace WebApp.Service.IService
{
  /// <summary>
  /// Define el contrato para generar contraseñas de longitud variable.
  /// </summary>
  public interface IPasswordGenerationStrategy
  {
    /// <summary>
    /// Genera una contraseña de una longitud especificada.
    /// </summary>
    /// <param name="length">La longitud de la contraseña a generar.</param>
    /// <returns>Una contraseña generada de la longitud especificada.</returns>
    string GeneratePassword(int length);
  }
}
