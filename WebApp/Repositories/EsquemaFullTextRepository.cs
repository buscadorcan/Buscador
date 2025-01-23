using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
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
