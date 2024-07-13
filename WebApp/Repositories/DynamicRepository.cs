using System.Data;
using Microsoft.EntityFrameworkCore;
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

        public DynamicRepository(
            IDbContextFactory dbContextFactory,
            ILogger<DynamicRepository> logger,
            ISqlServerDbContextFactory sqlServerDbContextFactory,
            IConectionStringBuilderService connectionStringBuilderService
        ) : base(sqlServerDbContextFactory, logger)
        {
            _connectionStringBuilderService = connectionStringBuilderService;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public List<PropiedadesTablaDto> GetProperties(int idSystem, string viewName)
        {
            try
            {
                var conexion = ExecuteDbOperation(context => context.Conexion.AsNoTracking().FirstOrDefault(u => u.IdSistema == idSystem));
                if (conexion == null)
                {
                    _logger.LogWarning($"No se encontró conexión para IdSistema {idSystem}");
                    throw new Exception($"No se encontró conexión para IdSistema {idSystem}");
                }

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
                    return new List<PropiedadesTablaDto>();
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las propiedades de la tabla");
                throw new Exception("Error al obtener las propiedades de la tabla", ex);
            }
        }

        private DbContext GetContext(Conexion conexion)
        {
            var connectionString = _connectionStringBuilderService.BuildConnectionString(conexion);
            return conexion.MotorBaseDatos switch
            {
                "MYSQL" => _dbContextFactory.CreateDbContext(connectionString, DatabaseType.MySql),
                "POSTGRES" => _dbContextFactory.CreateDbContext(connectionString, DatabaseType.PostgreSql),
                _ => _dbContextFactory.CreateDbContext(connectionString, DatabaseType.SqlServer)
            };
        }
    }
}
