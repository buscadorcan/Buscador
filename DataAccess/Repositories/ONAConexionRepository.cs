using System.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class ONAConexionRepository : BaseRepository, IONAConexionRepository
    {
        public ONAConexionRepository(
          ILogger<ONAConexionRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {

        }
        public bool Create(ONAConexion data)
        {
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
        public async Task<ONAConexion?> FindByIdONAAsync(int IdONA)
        {
            return await ExecuteDbOperation(async context =>
                await context.ONAConexion.AsNoTracking().FirstOrDefaultAsync(u => u.IdONA == IdONA)
            );
        }

        public List<ONAConexion> FindAll()
        {
            return ExecuteDbOperation(context => context.ONAConexion.AsNoTracking().Where(c => c.Estado.Equals("A")).ToList());
        }
        public List<ONAConexion> GetOnaConexionByOnaListAsync(int idOna)
        {
            return ExecuteDbOperation(context =>
                context.ONAConexion
                    .AsNoTracking()
                    .Where(c => c.IdONA == idOna && c.Estado == "A")
                    .ToList()
            );
        }

        public bool Update(ONAConexion newRecord, int userToken)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdONA == newRecord.IdONA);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = userToken;

                context.ONAConexion.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
      

    }
}
