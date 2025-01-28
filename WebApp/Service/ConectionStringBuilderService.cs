using SharedApp.Data;
using WebApp.Models;
using WebApp.Service.IService;

namespace WebApp.Service
{
    public class ConectionStringBuilderService : IConectionStringBuilderService
    {
        public string BuildConnectionString(ONAConexion conexion)
        {
            if (!Enum.TryParse(conexion.OrigenDatos, true, out DatabaseType databaseType))
            {
                databaseType = DatabaseType.SQLSERVER;
            }

            return databaseType switch
            {
                DatabaseType.SQLSERVER => BuildSqlServerConnectionString(conexion),
                DatabaseType.MYSQL => BuildMysqlConnectionString(conexion),
                DatabaseType.POSTGRES => BuildPostgresConnectionString(conexion),
                DatabaseType.SQLLITE => BuildSqliteConnectionString(conexion),
                _ => BuildSqlServerConnectionString(conexion)
            };
        }
        string BuildMysqlConnectionString(ONAConexion conexion)
        {
            string sslMode = "None"; // Puedes cambiar a "None", "Required", "VerifyCA" o "VerifyFull" según lo necesites.

            return $"Server={conexion.Host};" +
                   $"Database={conexion.BaseDatos};" +
                   $"Uid={conexion.Usuario};" +
                   $"Pwd={conexion.Contrasenia};" +
                   $"Port={conexion.Puerto};" +
                   $"SslMode={sslMode};"; // Incluye el modo SSL en la cadena de conexión
        }

        string BuildSqlServerConnectionString(ONAConexion conexion)
        {
            string portString = conexion.Puerto != 0 ? $",{conexion.Puerto}" : "";
            return $"Server={conexion.Host};Database={conexion.BaseDatos};User Id={conexion.Usuario};Password={conexion.Contrasenia};TrustServerCertificate=True;";
        }

        string BuildSqliteConnectionString(ONAConexion conexion)
        {
            return $"data source={conexion.BaseDatos}";
        }

        string BuildPostgresConnectionString(ONAConexion conexion)
        {
            return $"Host={conexion.Host};Port={conexion.Puerto};Database={conexion.BaseDatos};Username={conexion.Usuario};Password={conexion.Contrasenia};";
        }
    }
}
