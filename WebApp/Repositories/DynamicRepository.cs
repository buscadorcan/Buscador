using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SharedApp.Data;
using SharedApp.Models.Dtos;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class DynamicRepository : BaseRepository, IDynamicRepository
    {
        private readonly IConectionStringBuilderService _connectionStringBuilderService;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly ILogger<DynamicRepository> _logger;
        private readonly IMigrador _migrador;

        public DynamicRepository(
          IDbContextFactory dbContextFactory,
          ILogger<DynamicRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory,
          IConectionStringBuilderService connectionStringBuilderService,
          IMigrador migrador
        ) : base(sqlServerDbContextFactory, logger)
        {
            _connectionStringBuilderService = connectionStringBuilderService;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _migrador = migrador;
        }
        public List<PropiedadesTablaDto> GetProperties(int idONA, string viewName)
        {
            var conexion = GetConexion(idONA);
            using var context = GetContext(conexion);
            using var connection = context.Database.GetDbConnection();
            connection.Open();

            var columnNames = new List<PropiedadesTablaDto>();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT TOP 1 * FROM {viewName}";
            using var reader = command.ExecuteReader(CommandBehavior.SchemaOnly);
            var schemaTable = reader.GetSchemaTable();

            if (schemaTable == null)
            {
                _logger.LogWarning($"No se pudo obtener el esquema para la vista {viewName}");
                return columnNames;
            }

            foreach (DataRow row in schemaTable.Rows)
            {
                columnNames.Add(new PropiedadesTablaDto
                {
                    NombreColumna = row["ColumnName"].ToString()
                });
            }

            return columnNames;
        }
        public List<string> GetViewNames(int idONA)
        {
            var conexion = GetConexion(idONA);
            using var context = GetContext(conexion);
            using var connection = context.Database.GetDbConnection();
            connection.Open();

            var viewNames = new List<string>();
            using var command = connection.CreateCommand();

            switch (conexion.OrigenDatos)
            {
                case "MYSQL":
                    command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = DATABASE()";
                    break;
                case "POSTGRES":
                    command.CommandText = "SELECT table_name FROM information_schema.views WHERE table_schema = 'public'";
                    break;
                case "SQLITE":
                    command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'view'";
                    break;
                default:
                    command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS";
                    break;
            }

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                viewNames.Add(reader.GetString(0));
            }

            return viewNames;
        }
        private DbContext GetContext(ONAConexion conexion)
        {
            var connectionString = _connectionStringBuilderService.BuildConnectionString(conexion);
            return conexion.OrigenDatos switch
            {
                "MYSQL" => _dbContextFactory.CreateDbContext(connectionString, DatabaseType.MYSQL),
                _ => _dbContextFactory.CreateDbContext(connectionString, DatabaseType.SQLSERVER)
            };
        }
        public ONAConexion GetConexion(int idONA)
        {
            var conexion = ExecuteDbOperation(context => context.ONAConexion.AsNoTracking().FirstOrDefault(u => u.IdONA == idONA));
            if (conexion == null)
            {
                var message = $"No se encontró conexión para IdSistema {idONA}";
                _logger.LogWarning(message);
                throw new Exception(message);
            }
            return conexion;
        }
        public List<EsquemaVistaDto> GetListaValidacionEsquema(int idONA, int idEsquema)
        {
            return ExecuteDbOperation(context =>
            {
                // Obtener el IdEsquemaVista desde la tabla EsquemaVista
                var idEsquemaVista = context.EsquemaVista
                    .Where(v => v.IdONA == idONA && v.IdEsquema == idEsquema && v.Estado == "A")
                    .Select(v => v.IdEsquemaVista)
                    .FirstOrDefault();

                // Validar si se encontró un IdEsquemaVista
                if (idEsquemaVista == 0)
                {
                    return new List<EsquemaVistaDto>(); // Retornar una lista vacía si no se encuentra
                }

                // Consultar la tabla EsquemaVistaColumna con el IdEsquemaVista obtenido
                return (from c in context.EsquemaVistaColumna
                        where c.IdEsquemaVista == idEsquemaVista && c.Estado == "A"
                        select new EsquemaVistaDto
                        {
                            NombreEsquema = c.ColumnaEsquema,
                            NombreVista = c.ColumnaVista
                        }).ToList();
            });
        }

        public bool TestDatabaseConnection(ONAConexion conexion)
        {
            try
            {
                // Construir la cadena de conexión
                var connectionString = _connectionStringBuilderService.BuildConnectionString(conexion);

                // Crear conexión según el tipo de base de datos
                IDbConnection connection = conexion.OrigenDatos.ToUpper() switch
                {
                    "SQLSERVER" => new Microsoft.Data.SqlClient.SqlConnection(connectionString),
                    "MYSQL" => new MySql.Data.MySqlClient.MySqlConnection(connectionString),
                    "POSTGRES" => new Npgsql.NpgsqlConnection(connectionString),
                    "SQLITE" => new Microsoft.Data.Sqlite.SqliteConnection(connectionString),
                    _ => throw new NotSupportedException($"Tipo de base de datos '{conexion.OrigenDatos}' no soportado.")
                };

                // Abrir la conexión y verificar el estado
                connection.Open();
                return connection.State == ConnectionState.Open;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al probar la conexión: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MigrarConexionAsync(int idONA)
        {
            try
            {
                // Obtener la conexión desde la base de datos
                var conexion = GetConexion(idONA);

                if (conexion == null)
                {
                    _logger.LogWarning($"No se encontró conexión para IdONA {idONA}");
                    return false;
                }

                // Llamar al servicio `Migrador` para realizar la migración
                bool resultado = await _migrador.MigrarAsync(conexion);

                if (resultado)
                {
                    _logger.LogInformation($"Migración completada con éxito para IdONA {idONA}");
                }
                else
                {
                    _logger.LogWarning($"La migración no se pudo completar para IdONA {idONA}");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en la migración de la conexión {idONA}: {ex.Message}");
                return false;
            }
        }
    }
}
