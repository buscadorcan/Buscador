using SharedApp.Data;
using WebApp.Models;
using WebApp.Service.IService;

namespace WebApp.Service
{
    public class ConectionStringBuilderService : IConectionStringBuilderService
    {
        public string BuildConnectionString(Conexion conexion)
        {
            if (!Enum.TryParse(conexion.MotorBaseDatos, true, out DatabaseType databaseType))
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

        string BuildMysqlConnectionString(Conexion conexion)
        {
          return $"Server={conexion.Host};Port={conexion.Puerto};Database={conexion.BaseDatos};Uid={conexion.Usuario};Pwd={conexion.Contrasenia};";
        }

        string BuildSqlServerConnectionString(Conexion conexion)
        {
          string portString = conexion.Puerto != 0 ? $",{conexion.Puerto}" : "";
          return $"Server={conexion.Host}{portString};Database={conexion.BaseDatos};User Id={conexion.Usuario};Password={conexion.Contrasenia};TrustServerCertificate=True;";
        }

        string BuildSqliteConnectionString(Conexion conexion)
        {
            return $"data source={conexion.BaseDatos}";
        }

        string BuildPostgresConnectionString(Conexion conexion)
        {
            return $"Host={conexion.Host};Port={conexion.Puerto};Database={conexion.BaseDatos};Username={conexion.Usuario};Password={conexion.Contrasenia};";
        }
    }
}
