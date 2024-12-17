using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class HomologacionRepository : BaseRepository, IHomologacionRepository
  {
    private readonly IJwtService _jwtService;
    public HomologacionRepository(
      IJwtService jwtService,
      ILogger<UsuarioRepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
      _jwtService = jwtService;
    }
    public bool Create(Homologacion data)
    {
      data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
      data.IdUserModifica = data.IdUserCreacion;

      return ExecuteDbOperation(context => {
        context.Homologacion.Add(data);
        return context.SaveChanges() >= 0;
      });
    }
    public Homologacion? FindById(int id)
    {
      return ExecuteDbOperation(context => context.Homologacion.AsNoTracking().FirstOrDefault(u => u.IdHomologacion == id));
    }
    public Homologacion? FindByMostrarWeb(string? filter)
    {
      return ExecuteDbOperation(context => context.Homologacion.AsNoTracking().FirstOrDefault(u => u.MostrarWeb == filter));
    }
    public ICollection<Homologacion> FindByParent(int parentId)
    {
      return ExecuteDbOperation(context => context.Homologacion.Where(c => c.IdHomologacionGrupo == parentId && c.Estado.Equals("A")).OrderBy(c => c.MostrarWebOrden).ToList());
    }
    public List<Homologacion> FindByIds(int[] ids)
    {
      return ExecuteDbOperation(context => context.Homologacion.Where(c => ids.Contains(c.IdHomologacion)).OrderBy(c => c.MostrarWebOrden).ToList());
    }
    public bool Update(Homologacion newRecord)
    {
      return ExecuteDbOperation(context => {
        var _exits = MergeEntityProperties(context, newRecord, u => u.IdHomologacion == newRecord.IdHomologacion);

        _exits.FechaModifica = DateTime.Now;
        _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

        context.Homologacion.Update(_exits);
        return context.SaveChanges() >= 0;
      });
    }
  }
}
