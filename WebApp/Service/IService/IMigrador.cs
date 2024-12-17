using WebApp.Models;

namespace WebApp.Service.IService
{
  /// <summary>
  /// Define un contrato para la migración de datos utilizando una conexión de base de datos (ONAConexion).
  /// </summary>
  public interface IMigrador
  {
    /// <summary>
    /// Realiza la migración de datos desde o hacia una conexión de base de datos.
    /// </summary>
    /// <param name="conexion">La conexión de base de datos utilizada para la migración de datos.</param>
    /// <returns>Un valor booleano que indica si la migración fue exitosa.</returns>
    bool Migrar(ONAConexion conexion);
  }
}
