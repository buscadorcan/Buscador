using System.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class ONARepository : BaseRepository, IONARepository
    {
        public ONARepository(
          ILogger<ONARepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
            
        {
        }
        public bool Create(ONA data)
        {
            data.IdUserModifica = data.IdUserCreacion;
            data.InfoExtraJson = "{}";
            data.Estado = "A";

            return ExecuteDbOperation(context =>
            {
                context.ONA.Add(data);
                return context.SaveChanges() >= 0;
            });
        }
        public ONA? FindById(int id)
        {
            return ExecuteDbOperation(context => context.ONA.AsNoTracking().FirstOrDefault(u => u.IdONA == id));
        }
        public async Task<ONA?> FindByIdAsync(int id)
        {
            return await ExecuteDbOperation(async context =>
                await context.ONA.AsNoTracking().FirstOrDefaultAsync(u => u.IdONA == id)
            );
        }

        public ONA? FindBySiglas(string siglas)
        {
            return ExecuteDbOperation(context => context.ONA.AsNoTracking().FirstOrDefault(u => u.Siglas.Equals(siglas)));
        }
        public List<ONA> FindAll()
        {
            return ExecuteDbOperation(context => context.ONA.AsNoTracking().Where(c => c.Estado.Equals("A")).ToList());
        }
        public List<ONA> GetListByONAsAsync(int idOna)
        {
            return ExecuteDbOperation(context =>
                context.ONA
                    .AsNoTracking()
                    .Where(c => c.IdONA == idOna && c.Estado == "A")
                    .ToList()
            );
        }

        public List<VwPais> FindAllPaises()
        {
            return ExecuteDbOperation(context => context.VwPais.AsNoTracking().ToList());
        }
        public bool Update(ONA newRecord, int userToken)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdONA == newRecord.IdONA);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = userToken; 

                context.ONA.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }

    }
}
