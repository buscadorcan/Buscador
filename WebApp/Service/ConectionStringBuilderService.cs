using SharedApp.Data;
using WebApp.Models;
using WebApp.Service.IService;

namespace WebApp.Service
{
  public class ConectionStringBuilderService : IConectionStringBuilderService
  {
    /// <summary>
    /// Construye la cadena de conexión según el tipo de base de datos.
    /// </summary>
    /// <param name="conexion">El objeto que contiene los detalles de conexión a la base de datos.</param>
    /// <returns>La cadena de conexión generada para la base de datos especificada.</returns>
    public string BuildConnectionString(ONAConexion conexion)
    {
      if (!Enum.TryParse(conexion.OrigenDatos, true, out DatabaseType databaseType))
      {
        databaseType = DatabaseType.MSSQL;
      }

      return databaseType switch
      {
        DatabaseType.MSSQL => BuildSqlServerConnectionString(conexion),
        DatabaseType.MYSQL => BuildMysqlConnectionString(conexion),
        DatabaseType.POSTGRES => BuildPostgresConnectionString(conexion),
        DatabaseType.SQLITE => BuildSqliteConnectionString(conexion),
        _ => BuildSqlServerConnectionString(conexion)
      };
    }

    /// <summary>
    /// Genera la cadena de conexión para una base de datos MySQL.
    /// </summary>
    /// <param name="conexion">El objeto que contiene los detalles de conexión a la base de datos.</param>
    /// <returns>La cadena de conexión de MySQL.</returns>
    string BuildMysqlConnectionString(ONAConexion conexion)
    {
      return $"Server={conexion.Host};Port={conexion.Puerto};Database={conexion.BaseDatos};Uid={conexion.Usuario};Pwd={conexion.Contrasenia};";
    }

    /// <summary>
    /// Genera la cadena de conexión para una base de datos SQL Server.
    /// </summary>
    /// <param name="conexion">El objeto que contiene los detalles de conexión a la base de datos.</param>
    /// <returns>La cadena de conexión de SQL Server.</returns>
    string BuildSqlServerConnectionString(ONAConexion conexion)
    {
      string portString = conexion.Puerto != 0 ? $",{conexion.Puerto}" : "";
      return $"Server={conexion.Host}{portString};Database={conexion.BaseDatos};User Id={conexion.Usuario};Password={conexion.Contrasenia};TrustServerCertificate=True;";
    }

    /// <summary>
    /// Genera la cadena de conexión para una base de datos SQLite.
    /// </summary>
    /// <param name="conexion">El objeto que contiene los detalles de conexión a la base de datos.</param>
    /// <returns>La cadena de conexión de SQLite.</returns>
    string BuildSqliteConnectionString(ONAConexion conexion)
    {
      return $"data source={conexion.BaseDatos}";
    }

    /// <summary>
    /// Genera la cadena de conexión para una base de datos PostgreSQL.
    /// </summary>
    /// <param name="conexion">El objeto que contiene los detalles de conexión a la base de datos.</param>
    /// <returns>La cadena de conexión de PostgreSQL.</returns>
    string BuildPostgresConnectionString(ONAConexion conexion)
    {
      return $"Host={conexion.Host};Port={conexion.Puerto};Database={conexion.BaseDatos};Username={conexion.Usuario};Password={conexion.Contrasenia};";
    }
  }
}
