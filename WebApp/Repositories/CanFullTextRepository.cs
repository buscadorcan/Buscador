using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class CanFullTextRepository : BaseRepository, ICanFullTextRepository
  {
      public CanFullTextRepository(
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
      ) : base(sqlServerDbContextFactory, logger)
      {
      }

    public CanFullText Create(CanFullText data)
    {
        data.IdCanFullText = 0;

        return ExecuteDbOperation(context => {
            context.CanFullText.Add(data);
            context.SaveChanges();
            return data;
        });
    }

    public CanFullText? FindById(int id)
    {
      return ExecuteDbOperation(context => context.CanFullText.AsNoTracking().FirstOrDefault(u => u.IdCanFullText == id));
    }
  }
}