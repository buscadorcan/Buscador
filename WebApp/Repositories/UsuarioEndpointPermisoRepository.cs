using System.Data;
using WebApp.Repositories.IRepositories;
using WebApp.Models;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class UsuarioEndpointPermisoRepository : BaseRepository, IUsuarioEndpointPermisoRepository
    {
        private readonly IJwtService _jwtService;
        public UsuarioEndpointPermisoRepository (
            IJwtService jwtService,
            ILogger<EndpointRepository> logger,
            IDbContextFactory dbContextFactory
        ) : base(dbContextFactory, logger)
        {
            _jwtService = jwtService;
        }
        public UsuarioEndpointPermiso? FindByEndpointId(int endpointId)
        {
            return ExecuteDbOperation(context => context.UsuarioEndpointPermiso.FirstOrDefault(c => c.IdEndpoint == endpointId));
        }

        public ICollection<UsuarioEndpointPermiso> FindAll()
        {
            return ExecuteDbOperation(context => context.UsuarioEndpointPermiso.OrderBy(c => c.IdUsuarioEndpointPermiso).ToList());
        }

        public bool Create(UsuarioEndpointPermiso usuarioEndpointPermiso)
        {
            usuarioEndpointPermiso.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            usuarioEndpointPermiso.IdUserModifica = usuarioEndpointPermiso.IdUserCreacion;

            return ExecuteDbOperation(context =>
            {
                context.UsuarioEndpointPermiso.Add(usuarioEndpointPermiso);
                return context.SaveChanges() >= 0;
            });
        }
    }
}