using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Npgsql;
using SharedApp.Models.Dtos;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class ONAConexionRepository : BaseRepository, IONAConexionRepository
    {
        private readonly IJwtService _jwtService;
        private readonly IMigrador _iMigrador;
        public ONAConexionRepository(
          IJwtService jwtService,
          ILogger<ONAConexionRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory,
          IMigrador iMigrador
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
            _iMigrador = iMigrador;

        }
        public bool Create(ONAConexion data)
        {
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            data.IdUserModifica = data.IdUserCreacion;
            data.FechaCreacion = DateTime.Now;
            data.FechaModifica = data.FechaCreacion;
            data.Estado = "A";

            return ExecuteDbOperation(context =>
            {
                context.ONAConexion.Add(data);
                return context.SaveChanges() >= 0;
            });
        }
        public ONAConexion? FindById(int id)
        {
            return ExecuteDbOperation(context => context.ONAConexion.AsNoTracking().FirstOrDefault(u => u.IdONA == id));
        }
        public ONAConexion? FindByIdONA(int IdONA)
        {
            return ExecuteDbOperation(context => context.ONAConexion.AsNoTracking().FirstOrDefault(u => u.IdONA == IdONA));
        }
        public List<ONAConexion> FindAll()
        {
            return ExecuteDbOperation(context => context.ONAConexion.AsNoTracking().Where(c => c.Estado.Equals("A")).ToList());
        }
        public bool Update(ONAConexion newRecord)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdONA == newRecord.IdONA);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

                context.ONAConexion.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
        public bool TestConnection(ONAConexion onoConexion)
        {
            try
            {
                ONAConexion objCone = new ONAConexion
                {
                    IdONA = onoConexion.IdONA,
                    Host = onoConexion.Host,
                    Puerto = onoConexion.Puerto,
                    Usuario = onoConexion.Usuario,
                    Contrasenia = onoConexion.Contrasenia,
                    BaseDatos = onoConexion.BaseDatos,
                    OrigenDatos = onoConexion.OrigenDatos,
                    Migrar = onoConexion.Migrar,
                    Estado = onoConexion.Estado
                };

                // Crear la cadena de conexión
                string connectionString = BuildConnectionString(objCone);

                // Crear la conexión según el tipo de motor de base de datos
                using (IDbConnection connection = CreateConnection(onoConexion.OrigenDatos.ToUpper(), connectionString))
                {
                    connection.Open();  // Abrir la conexión
                    if (connection.State == ConnectionState.Open)
                    {
                        Console.WriteLine("Conexión establecida correctamente.");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("No se pudo abrir la conexión.");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al probar la conexión: {ex.Message}");
                return false;
            }
        }

        private IDbConnection CreateConnection(string tipoBaseDatos, string connectionString)
        {
            IDbConnection connection = null;

            switch (tipoBaseDatos.ToUpper())
            {
                case "SQLSERVER":
                    connection = new SqlConnection(connectionString);
                    break;
                case "MYSQL":
                    connection = new MySqlConnection(connectionString);
                    break;
                case "POSTGRES":
                    connection = new NpgsqlConnection(connectionString);
                    break;
                //case "SQLITE":
                //    connection = new SQLiteConnection(connectionString);
                //    break;
                default:
                    throw new NotSupportedException($"Tipo de base de datos '{tipoBaseDatos}' no soportado.");
            }

            return connection;
        }

        public string BuildConnectionString(ONAConexion onoConexion)
        {
            switch (onoConexion.OrigenDatos.ToUpper())
            {
                case "SQLSERVER":
                    return $"Server={onoConexion.Host},{onoConexion.Puerto};Database={onoConexion.BaseDatos};User Id={onoConexion.Usuario};Password={onoConexion.Contrasenia};";
                case "MYSQL":
                    return $"Server={onoConexion.Host};Port={onoConexion.Puerto};Database={onoConexion.BaseDatos};Uid={onoConexion.Usuario};Pwd={onoConexion.Contrasenia};";
                case "POSTGRES":
                    return $"Host={onoConexion.Host};Port={onoConexion.Puerto};Database={onoConexion.BaseDatos};Username={onoConexion.Usuario};Password={onoConexion.Contrasenia};";
                case "SQLITE":
                    return $"Data Source={onoConexion.BaseDatos};Version=3;";
                default:
                    throw new NotSupportedException($"Tipo de base de datos '{onoConexion.OrigenDatos}' no soportado.");
            }
        }

        public async Task<bool> OnaMigracion(ONAConexion oNAConexion) 
        {
            bool exito = false;
            exito = await _iMigrador.MigrarAsync(oNAConexion);
            return exito;

        }

    }
}
