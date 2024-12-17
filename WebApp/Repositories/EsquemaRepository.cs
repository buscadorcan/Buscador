using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class EsquemaRepository : BaseRepository, IEsquemaRepository
  {
    private readonly IJwtService _jwtService;
    public EsquemaRepository(
      IJwtService jwtService,
      ILogger<UsuarioRepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
      _jwtService = jwtService;
    }    
    public bool Create(Esquema data)
    {
      data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
      data.IdUserModifica = data.IdUserCreacion;

      return ExecuteDbOperation(context => {
        context.Esquema.Add(data);
        return context.SaveChanges() >= 0;
      });
    }
    public Esquema? FindById(int id)
    {
      return ExecuteDbOperation(context => context.Esquema.AsNoTracking().FirstOrDefault(u => u.IdEsquema == id));
    }
    public Esquema? FindByViewName(string esquemaVista)
    {
        return ExecuteDbOperation(context => context.Esquema.AsNoTracking().FirstOrDefault(u => u.EsquemaVista == esquemaVista));
    }
    public List<Esquema> FindAll()
    {
      return ExecuteDbOperation(context => context.Esquema.AsNoTracking().Where(c => c.Estado.Equals("A")).OrderBy(c => c.MostrarWebOrden).ToList());
    }
    public List<Esquema> FindAllWithViews()
    {
      return ExecuteDbOperation(context => context.Esquema.AsNoTracking().Where(c => c.Estado == "A" && c.EsquemaVista != null && !string.IsNullOrEmpty(c.EsquemaVista.Trim())).OrderBy(c => c.MostrarWebOrden).ToList());
    }
    public bool Update(Esquema newRecord)
    {
      return ExecuteDbOperation(context => {
        var _exits = MergeEntityProperties(context, newRecord, u => u.IdEsquema == newRecord.IdEsquema);

        _exits.FechaModifica = DateTime.Now;
        _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

        context.Esquema.Update(_exits);
        return context.SaveChanges() >= 0;
      });
    }
  }
}
