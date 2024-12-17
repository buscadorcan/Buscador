namespace WebApp.Service.IService
{
  /// <summary>
  /// Define un contrato para un servicio encargado de generar valores hash a partir de una entrada de texto.
  /// </summary>
  public interface IHashService
  {
    /// <summary>
    /// Genera un valor hash a partir de una cadena de texto proporcionada.
    /// </summary>
    /// <param name="input">La cadena de texto que se desea hashear. Puede ser nula.</param>
    /// <returns>El valor hash generado como una cadena de texto.</returns>
    string GenerateHash(string? input);
  }
}
