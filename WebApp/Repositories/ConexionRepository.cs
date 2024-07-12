using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class ConexionRepository : BaseRepository, IConexionRepository
  {
      private readonly IJwtService _jwtService;
       public ConexionRepository(
            IJwtService jwtService,
            ILogger<ConexionRepository> logger,
            IDbContextFactory dbContextFactory
        ) : base(dbContextFactory, logger)
        {
            _jwtService = jwtService;
        }
        public bool Create(Conexion data)
        {
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            data.IdUserModifica = data.IdUserCreacion;

            return ExecuteDbOperation(context =>
            {
                context.Conexion.Add(data);
                return context.SaveChanges() >= 0;
            });
        }
        public Conexion? FindById(int id)
        {
            return ExecuteDbOperation(context => context.Conexion.AsNoTracking().FirstOrDefault(u => u.IdConexion == id));
        }
        public List<Conexion> FindAll()
        {
            return ExecuteDbOperation(context => context.Conexion.AsNoTracking().Where(c => c.Estado.Equals("A")).OrderBy(c => c.FechaCreacion).ToList());
        }
        public bool Update(Conexion newRecord)
        {
          return ExecuteDbOperation(context => {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdConexion == newRecord.IdConexion);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

                context.Conexion.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
    }
}