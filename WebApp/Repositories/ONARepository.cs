using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class ONARepository : BaseRepository, IONARepository
  {
    private readonly IJwtService _jwtService;
    public ONARepository(
      IJwtService jwtService,
      ILogger<ONARepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
      _jwtService = jwtService;
    }
    public bool Create(ONA data)
    {
      data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
      data.IdUserModifica = data.IdUserCreacion;

      return ExecuteDbOperation(context => {
        context.ONA.Add(data);
        return context.SaveChanges() >= 0;
      });
    }
    public ONA? FindById(int id)
    {
      return ExecuteDbOperation(context => context.ONA.AsNoTracking().FirstOrDefault(u => u.IdONA == id));
    }
    public ONA? FindBySiglas(string siglas)
    {
      return ExecuteDbOperation(context => context.ONA.AsNoTracking().FirstOrDefault(u => u.Siglas.Equals(siglas)));
    }
    public List<ONA> FindAll()
    {
      return ExecuteDbOperation(context => context.ONA.AsNoTracking().Where(c => c.Estado.Equals("A")).OrderBy(c => c.FechaCreacion).ToList());
    }
    public bool Update(ONA newRecord)
    {
      return ExecuteDbOperation(context => {
        var _exits = MergeEntityProperties(context, newRecord, u => u.IdONA == newRecord.IdONA);

        _exits.FechaModifica = DateTime.Now;
        _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

        context.ONA.Update(_exits);
        return context.SaveChanges() >= 0;
      });
    }
  }
}
