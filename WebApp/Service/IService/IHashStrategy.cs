namespace WebApp.Service.IService
{
  /// <summary>
  /// Define un contrato para una estrategia de generación de valores hash a partir de una cadena de texto.
  /// </summary>
  public interface IHashStrategy
  {
    /// <summary>
    /// Calcula un valor hash a partir de la cadena de texto proporcionada.
    /// </summary>
    /// <param name="input">La cadena de texto que se desea hashear. Puede ser nula.</param>
    /// <returns>El valor hash generado como una cadena de texto.</returns>
    string ComputeHash(string? input);
  }
}
