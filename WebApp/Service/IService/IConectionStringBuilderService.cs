using WebApp.Models;

namespace WebApp.Service.IService
{
  /// <summary>
  /// Define un contrato para la construcción de cadenas de conexión utilizando un objeto <see cref="ONAConexion"/>.
  /// </summary>
  public interface IConectionStringBuilderService
  {
    /// <summary>
    /// Construye una cadena de conexión a partir de los parámetros proporcionados por un objeto <see cref="ONAConexion"/>.
    /// </summary>
    /// <param name="conexion">El objeto <see cref="ONAConexion"/> que contiene la información para construir la cadena de conexión.</param>
    /// <returns>Una cadena de conexión construida con los datos proporcionados.</returns>
    string BuildConnectionString(ONAConexion conexion);
  }
}
