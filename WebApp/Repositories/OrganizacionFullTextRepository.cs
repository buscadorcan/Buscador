using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class OrganizacionFullTextRepository : BaseRepository, IOrganizacionFullTextRepository
  {
      public OrganizacionFullTextRepository(
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
      ) : base(sqlServerDbContextFactory, logger)
      {
      }

    public OrganizacionFullText Create(OrganizacionFullText data)
    {
        data.IdOrganizacionFullText = 0;

        return ExecuteDbOperation(context => {
            context.OrganizacionFullText.Add(data);
            context.SaveChanges();
            return data;
        });
    }

    public OrganizacionFullText? FindById(int id)
    {
      return ExecuteDbOperation(context => context.OrganizacionFullText.AsNoTracking().FirstOrDefault(u => u.IdOrganizacionFullText == id));
    }
  }
}