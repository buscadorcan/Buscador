using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccess.Models;

namespace DataAccess.Repositories
{
  public class EsquemaFullTextRepository : BaseRepository, IEsquemaFullTextRepository
  {
    public EsquemaFullTextRepository(
      ILogger<UsuarioRepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
    }
    public EsquemaFullText Create(EsquemaFullText data)
    {
      data.IdEsquemaFullText = 0;

      return ExecuteDbOperation(context => {
        context.EsquemaFullText.Add(data);
        context.SaveChanges();
        return data;
      });
    }
        public async Task<EsquemaFullText> CreateAsync(EsquemaFullText data)
        {
            data.IdEsquemaFullText = 0;

            return ExecuteDbOperation(context => {
                context.EsquemaFullText.Add(data);
                context.SaveChanges();
                return data;
            });
        }
        public EsquemaFullText? FindById(int id)
    {
      return ExecuteDbOperation(context => context.EsquemaFullText.AsNoTracking().FirstOrDefault(u => u.IdEsquemaFullText == id));
    }
  }
}
