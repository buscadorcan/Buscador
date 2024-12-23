using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class EsquemaVistaColumnaRepository : BaseRepository, IEsquemaVistaColumnaRepository
  {
    private readonly IJwtService _jwtService;
    public EsquemaVistaColumnaRepository(
      IJwtService jwtService,
      ILogger<UsuarioRepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
      _jwtService = jwtService;
    }    
    public bool Create(EsquemaVistaColumna data)
    {
      data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
      data.IdUserModifica = data.IdUserCreacion;

      return ExecuteDbOperation(context => {
        context.EsquemaVistaColumna.Add(data);
        return context.SaveChanges() >= 0;
      });
    }
    public EsquemaVistaColumna? FindById(int id)
    {
      return ExecuteDbOperation(context => context.EsquemaVistaColumna.AsNoTracking().FirstOrDefault(u => u.IdEsquemaVistaColumna == id));
    }
    public List<EsquemaVistaColumna> FindByIdEsquemaVista(int IdEsquemaVista)
    {
      return ExecuteDbOperation(context => context.EsquemaVistaColumna.AsNoTracking().Where(u => u.IdEsquemaVista == IdEsquemaVista).ToList());
    }
    public List<EsquemaVistaColumna> FindAll()
    {
      return ExecuteDbOperation(context => context.EsquemaVistaColumna.AsNoTracking().Where(c => c.Estado.Equals("A")).ToList());
    }
    public bool Update(EsquemaVistaColumna newRecord)
    {
      return ExecuteDbOperation(context => {
        var _exits = MergeEntityProperties(context, newRecord, u => u.IdEsquemaVistaColumna == newRecord.IdEsquemaVistaColumna);

        _exits.FechaModifica = DateTime.Now;
        _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

        context.EsquemaVistaColumna.Update(_exits);
        return context.SaveChanges() >= 0;
      });
    }
    
  }
}
