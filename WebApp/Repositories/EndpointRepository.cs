using System.Data;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class EndpointRepository : BaseRepository, IEndpointRepository
    {
        private readonly IJwtService _jwtService;
        public EndpointRepository (
            IJwtService jwtService,
            ILogger<EndpointRepository> logger,
            IDbContextFactory dbContextFactory
        ) : base(dbContextFactory, logger)
        {
            _jwtService = jwtService;
        }
        public Models.Endpoint? FindById(int idEndpoint)
        {
            return ExecuteDbOperation(context => context.Endpoint.FirstOrDefault(c => c.IdEndpoint == idEndpoint));
        }
        public ICollection<Models.Endpoint> FindAll()
        {
            return ExecuteDbOperation(context => context.Endpoint.OrderBy(c => c.IdEndpoint).ToList());
        }
        public bool Create(Models.Endpoint endpoint)
        {
            endpoint.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            endpoint.IdUserModifica = endpoint.IdUserCreacion;

            return ExecuteDbOperation(context =>
            {
                context.Endpoint.Add(endpoint);
                return context.SaveChanges() >= 0;
            });
        }
        public bool IsUniqueUserUrl(string nombre, string url)
        {
            return ExecuteDbOperation(context => {
                return context.Endpoint.FirstOrDefault(u => u.Nombre == nombre && u.Url == url) == null;
            });
        }
    }
}