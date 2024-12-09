using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class LogMigracionRepository : BaseRepository, ILogMigracionRepository
  {
    private readonly IJwtService _jwtService;
    public LogMigracionRepository(
      IJwtService jwtService,
      ILogger<LogMigracionRepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
      _jwtService = jwtService;
    }
    public LogMigracion Create(LogMigracion data)
    {
      return ExecuteDbOperation(context =>
      {
        context.LogMigracion.Add(data);
        context.SaveChanges();
        return data;
      });
    }
    public LogMigracion? FindById(int id)
    {
      return ExecuteDbOperation(context => context.LogMigracion.AsNoTracking().FirstOrDefault(u => u.IdLogMigracion == id));
    }
    public List<LogMigracion> FindAll()
    {
      return ExecuteDbOperation(context => context.LogMigracion.AsNoTracking().OrderByDescending(c => c.Fecha).ToList());
    }
    public bool Update(LogMigracion newRecord)
    {
      return ExecuteDbOperation(context => {
        var _exits = MergeEntityProperties(context, newRecord, u => u.IdLogMigracion == newRecord.IdLogMigracion);

        context.LogMigracion.Update(_exits);
        return context.SaveChanges() >= 0;
      });
    }
    public LogMigracionDetalle CreateDetalle(LogMigracionDetalle data)
    {
      return ExecuteDbOperation(context =>
      {
        context.LogMigracionDetalle.Add(data);
        context.SaveChanges();
        return data;
      });
    }
    public List<LogMigracionDetalle> FindAllDetalle()
    {
      return ExecuteDbOperation(context => context.LogMigracionDetalle.AsNoTracking().OrderByDescending(c => c.Fecha).ToList());
    }
    public List<LogMigracionDetalle> FindDetalleById(int id)
    {
      return ExecuteDbOperation(context => context.LogMigracionDetalle.AsNoTracking().Where(u => u.IdLogMigracion == id).ToList());
    }
    public bool UpdateDetalle(LogMigracionDetalle newRecord)
    {
      return ExecuteDbOperation(context => {
        var _exits = MergeEntityProperties(context, newRecord, u => u.IdLogMigracionDetalle == newRecord.IdLogMigracionDetalle);

        context.LogMigracionDetalle.Update(_exits);
        return context.SaveChanges() >= 0;
      });
    }
  }
}