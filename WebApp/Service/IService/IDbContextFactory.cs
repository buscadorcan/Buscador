using Microsoft.EntityFrameworkCore;
using SharedApp.Data;

namespace WebApp.Service.IService
{
  /// <summary>
  /// Define un contrato para la creación de instancias de DbContext utilizando una cadena de conexión y un tipo de base de datos.
  /// </summary>
  public interface IDbContextFactory
  {
     /// <summary>
     /// Crea una instancia de DbContext utilizando la cadena de conexión y el tipo de base de datos proporcionados.
     /// </summary>
     /// <param name="connectionString">La cadena de conexión que se usará para conectar a la base de datos.</param>
     /// <param name="databaseType">El tipo de base de datos (por ejemplo, SQL Server, MySQL, PostgreSQL) que se utilizará.</param>
     /// <returns>Una instancia de DbContext configurada para la base de datos especificada.</returns>
     DbContext CreateDbContext(string connectionString, DatabaseType databaseType);
  }
}
