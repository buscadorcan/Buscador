using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class HomologacionEsquemaRepository : BaseRepository, IHomologacionEsquemaRepository
  {
      private readonly IJwtService _jwtService;
       public HomologacionEsquemaRepository(
            IJwtService jwtService,
            ILogger<UsuarioRepository> logger,
            IDbContextFactory dbContextFactory
        ) : base(dbContextFactory, logger)
        {
            _jwtService = jwtService;
        }
        public bool Create(HomologacionEsquema data)
        {
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            data.IdUserModifica = data.IdUserCreacion;

            return ExecuteDbOperation(context =>
            {
                context.HomologacionEsquema.Add(data);
                return context.SaveChanges() >= 0;
            });
        }
        public HomologacionEsquema? FindById(int id)
        {
            return ExecuteDbOperation(context => context.HomologacionEsquema.AsNoTracking().FirstOrDefault(u => u.IdHomologacionEsquema == id));
        }
        public List<HomologacionEsquema> FindAll()
        {
            return ExecuteDbOperation(context => context.HomologacionEsquema.AsNoTracking().Where(c => c.Estado.Equals("A")).OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public bool Update(HomologacionEsquema newRecord)
        {
          return ExecuteDbOperation(context => {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdHomologacionEsquema == newRecord.IdHomologacionEsquema);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

                context.HomologacionEsquema.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
    }
}