using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class DataLakeRepository : BaseRepository, IDataLakeRepository
  {
      public DataLakeRepository (
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
      ) : base(sqlServerDbContextFactory, logger)
      {

      }

    public DataLake Create(DataLake data)
    {
        data.IdDataLake = 0;

        return ExecuteDbOperation(context =>
        {
            context.DataLake.Add(data);
            context.SaveChanges();
            context.Entry(data).State = EntityState.Detached;
            return data;
        });
    }

    public DataLake? FindById(int Id)
    {
      return ExecuteDbOperation(context => context.DataLake.AsNoTracking().FirstOrDefault(u => u.IdDataLake == Id));
    }

    public ICollection<DataLake> FindAll()
    {
      return ExecuteDbOperation(context => context.DataLake.AsNoTracking().Where(c => c.Estado != null && c.Estado.Equals("A")).OrderBy(c => c.DataFechaCarga).ToList());
    }

    public DataLake? FindBy(DataLake dataLake)
    {
      return ExecuteDbOperation(context => {
        return context.DataLake.AsNoTracking().FirstOrDefault(
                  u => u.DataTipo == dataLake.DataTipo &&
                  u.DataSistemaOrigen == dataLake.DataSistemaOrigen &&
                  u.DataSistemaOrigenId == dataLake.DataSistemaOrigenId
              );
      });
    }

    public DataLake? Update(DataLake newRecord)
    {
        return ExecuteDbOperation(context => {
            var _exits = MergeEntityProperties(context, newRecord, u => u.IdDataLake == newRecord.IdDataLake);

            _exits.DataSistemaFecha = DateTime.Now;

            context.DataLake.Update(_exits);
            context.SaveChanges();

            context.Entry(_exits).State = EntityState.Detached;

            return _exits;
        });
    }
  }
}