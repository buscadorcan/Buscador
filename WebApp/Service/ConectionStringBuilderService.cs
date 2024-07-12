using WebApp.Models;
using WebApp.Service.IService;

namespace WebApp.Service
{
    public class ConectionStringBuilderService : IConectionStringBuilderService
    {
        public string BuildConnectionString(Conexion conexion)
        {
            return conexion.MotorBaseDatos switch
            {
                "MSSQL" => BuildSqlServerConnectionString(conexion),
                "MYSQL" => BuildMysqlConnectionString(conexion),
                "POSTGRES" => BuildPostgresConnectionString(conexion),
                "SQLITE" => BuildSqliteConnectionString(conexion),
                _ => BuildSqlServerConnectionString(conexion)
            };
        }

        string BuildMysqlConnectionString(Conexion conexion)
        {
            return $"Server={conexion.Host};Port={conexion.Puerto};Database={conexion.BaseDatos};Uid={conexion.Usuario};Pwd={conexion.Contrasenia};";
        }

        string BuildSqlServerConnectionString(Conexion conexion)
        {
            return $"Server={conexion.Host},{conexion.Puerto};Database={conexion.BaseDatos};User Id={conexion.Usuario};Password={conexion.Contrasenia};TrustServerCertificate=True;";
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