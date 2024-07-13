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
            var conexion = GetConexion(idSystem);
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

        public List<string> GetViewNames(int idSystem)
        {
            var conexion = GetConexion(idSystem);
            using var context = GetContext(conexion);
            using var connection = context.Database.GetDbConnection();
            connection.Open();

            var viewNames = new List<string>();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS";
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                viewNames.Add(reader.GetString(0));
            }

            return viewNames;
        }

        private DbContext GetContext(Conexion conexion)
        {
            var connectionString = _connectionStringBuilderService.BuildConnectionString(conexion);
            return conexion.MotorBaseDatos switch
            {
                "MYSQL" => _dbContextFactory.CreateDbContext(connectionString, DatabaseType.MySql),
                _ => _dbContextFactory.CreateDbContext(connectionString, DatabaseType.SqlServer)
            };
        }

        private Conexion GetConexion(int idSystem)
        {
            var conexion = ExecuteDbOperation(context => context.Conexion.AsNoTracking().FirstOrDefault(u => u.IdSistema == idSystem));
            if (conexion == null)
            {
                var message = $"No se encontró conexión para IdSistema {idSystem}";
                _logger.LogWarning(message);
                throw new Exception(message);
            }

            return conexion;
        }
    }
}
