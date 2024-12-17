using System.Data;
using WebApp.Repositories.IRepositories;
using WebApp.Models;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class UsuarioEndpointRepository : BaseRepository, IUsuarioEndpointRepository
  {
    private readonly IJwtService _jwtService;
    public UsuarioEndpointRepository (
      IJwtService jwtService,
      ILogger<UsuarioEndpointRepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
      _jwtService = jwtService;
    }
    public UsuarioEndpoint? FindByEndpointId(int endpointId)
    {
      return ExecuteDbOperation(context => context.UsuarioEndpoint.FirstOrDefault(c => c.IdUsuarioEndPoint == endpointId));
    }
    public ICollection<UsuarioEndpoint> FindAll()
    {
      return ExecuteDbOperation(context => context.UsuarioEndpoint.OrderBy(c => c.IdUsuarioEndPoint).ToList());
    }
    public bool Create(UsuarioEndpoint usuarioEndpoint)
    {
      usuarioEndpoint.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
      usuarioEndpoint.IdUserModifica = usuarioEndpoint.IdUserCreacion;

      return ExecuteDbOperation(context => {
        context.UsuarioEndpoint.Add(usuarioEndpoint);
        return context.SaveChanges() >= 0;
      });
    }
  }
}
