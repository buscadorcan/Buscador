using System.Data;
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
        columnNames.Add(new PropiedadesTablaDto {
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
      return conexion.OrigenDatos switch {
        "MYSQL" => _dbContextFactory.CreateDbContext(connectionString, DatabaseType.MYSQL),
        _ => _dbContextFactory.CreateDbContext(connectionString, DatabaseType.SQLSERVER)
      };
    }
    private ONAConexion GetConexion(int idONA)
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

    public List<EsquemaVistaDto> GetListaValidacionEsquema(int idONA, int idEsquemaVista)
    {
        return ExecuteDbOperation(context =>
            (from c in context.EsquemaVistaColumna
             join v in context.EsquemaVista on c.IdEsquemaVista equals v.IdEsquemaVista
             where v.IdONA == idONA && v.Estado == "A" && v.IdEsquemaVista == idEsquemaVista
             select new EsquemaVistaDto
             {
                 NombreEsquema = c.ColumnaEsquema,
                 NombreVista = c.ColumnaVista
             }).ToList()
        );
    }
  }
}
