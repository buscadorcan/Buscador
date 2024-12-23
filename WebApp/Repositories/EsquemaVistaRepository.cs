using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class EsquemaVistaRepository : BaseRepository, IEsquemaVistaRepository
  {
    private readonly IJwtService _jwtService;
    public EsquemaVistaRepository(
      IJwtService jwtService,
      ILogger<UsuarioRepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
      _jwtService = jwtService;
    }    
    public bool Create(EsquemaVista data)
    {
      data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
      data.IdUserModifica = data.IdUserCreacion;

      return ExecuteDbOperation(context => {
        context.EsquemaVista.Add(data);
        return context.SaveChanges() >= 0;
      });
    }
    public EsquemaVista? FindById(int id)
    {
      return ExecuteDbOperation(context => context.EsquemaVista.AsNoTracking().FirstOrDefault(u => u.IdEsquemaVista == id));
    }
    public EsquemaVista? FindByIdEsquema(int idEsquema)
    {
        return ExecuteDbOperation(context => context.EsquemaVista.AsNoTracking().FirstOrDefault(u => u.IdEsquema == idEsquema));
    }
    public List<EsquemaVista> FindAll()
    {
      return ExecuteDbOperation(context => context.EsquemaVista.AsNoTracking().Where(c => c.Estado.Equals("A")).ToList());
    }
    public bool Update(EsquemaVista newRecord)
    {
      return ExecuteDbOperation(context => {
        var _exits = MergeEntityProperties(context, newRecord, u => u.IdEsquemaVista == newRecord.IdEsquemaVista);

        _exits.FechaModifica = DateTime.Now;
        _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

        context.EsquemaVista.Update(_exits);
        return context.SaveChanges() >= 0;
      });
    }

  }
}
