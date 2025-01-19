using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SharedApp.Models.Dtos;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class ONAConexionRepository : BaseRepository, IONAConexionRepository
    {
        private readonly IJwtService _jwtService;
        public ONAConexionRepository(
          IJwtService jwtService,
          ILogger<ONAConexionRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
        }
        public bool Create(ONAConexion data)
        {
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            data.IdUserModifica = data.IdUserCreacion;
            data.FechaCreacion = DateTime.Now;
            data.FechaModifica = data.FechaCreacion;
            data.Estado = "A";

            return ExecuteDbOperation(context =>
            {
                context.ONAConexion.Add(data);
                return context.SaveChanges() >= 0;
            });
        }
        public ONAConexion? FindById(int id)
        {
            return ExecuteDbOperation(context => context.ONAConexion.AsNoTracking().FirstOrDefault(u => u.IdONA == id));
        }
        public ONAConexion? FindByIdONA(int IdONA)
        {
            return ExecuteDbOperation(context => context.ONAConexion.AsNoTracking().FirstOrDefault(u => u.IdONA == IdONA));
        }
        public List<ONAConexion> FindAll()
        {
            return ExecuteDbOperation(context => context.ONAConexion.AsNoTracking().Where(c => c.Estado.Equals("A")).OrderBy(c => c.FechaCreacion).ToList());
        }
        public bool Update(ONAConexion newRecord)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdONA == newRecord.IdONA);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

                context.ONAConexion.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
        public ONAConexion? GetOnaConexionByOnaAsync(int idOna)
        {
            return ExecuteDbOperation(context => context.ONAConexion.AsNoTracking().FirstOrDefault(u => u.IdONA == idOna));
        }

        public bool TestConnection(ONAConexion onoConexion)
        {
           
            return true;
        }

     
    }
}
