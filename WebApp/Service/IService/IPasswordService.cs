namespace WebApp.Service.IService
{
  /// <summary>
  /// Define el contrato para la generación de contraseñas temporales.
  /// </summary>
  public interface IPasswordService
  {
    /// <summary>
    /// Genera una contraseña temporal de una longitud especificada.
    /// </summary>
    /// <param name="length">La longitud de la contraseña temporal a generar.</param>
    /// <returns>Una contraseña temporal generada de la longitud especificada.</returns>
    string GenerateTemporaryPassword(int length);
  }
}
