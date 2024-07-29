using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class DataLakeOrganizacionRepository : BaseRepository, IDataLakeOrganizacionRepository
  {
      public DataLakeOrganizacionRepository(
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
      ) : base(sqlServerDbContextFactory, logger)
      {
      }

    public DataLakeOrganizacion? Create(DataLakeOrganizacion data)
    {
      data.IdDataLakeOrganizacion = 0;

      try {
        return ExecuteDbOperation(context => {
            context.DataLakeOrganizacion.Add(data);
            context.SaveChanges();
            return data;
        });
      } catch (Exception e) {
        Console.WriteLine(e);
        return null;
      }
    }

    public DataLakeOrganizacion? FindById(int Id)
    {
      return ExecuteDbOperation(context => context.DataLakeOrganizacion.AsNoTracking().FirstOrDefault(u => u.IdDataLakeOrganizacion == Id));
    }

    public ICollection<DataLakeOrganizacion> FindAll()
    {
      return ExecuteDbOperation(context => context.DataLakeOrganizacion.AsNoTracking().Where(c => c.Estado != null && c.Estado.Equals("A", StringComparison.Ordinal)).OrderBy(c => c.IdDataLakeOrganizacion).ToList());
    }

    public bool Update(DataLakeOrganizacion newRecord)
    {
      return ExecuteDbOperation(context => {
          var currentRecord = MergeEntityProperties(context, newRecord, u => u.IdDataLakeOrganizacion == newRecord.IdDataLake);
          context.DataLakeOrganizacion.Update(currentRecord);
          return context.SaveChanges() >= 0;
      });
    }

    public int GetLastId()
    {
      return ExecuteDbOperation(context => context.DataLakeOrganizacion.AsNoTracking().Max(c => c.IdDataLakeOrganizacion));
    }

    public bool DeleteOldRecords(int IdHomologacionEsquema)
    {
      return ExecuteDbOperation(context => {
        var records = context.DataLakeOrganizacion.Where(c => c.IdHomologacionEsquema == IdHomologacionEsquema).ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdDataLakeOrganizacion).ToList();

        var deletedOrganizacionFullTextRecords = context.OrganizacionFullText.Where(o => deletedRecordIds.Contains(o.IdDataLakeOrganizacion)).ToList();
        context.OrganizacionFullText.RemoveRange(deletedOrganizacionFullTextRecords);
        context.SaveChanges();

        context.DataLakeOrganizacion.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }

    public bool DeleteOldRecord(string idVista, string idOrganizacion, List<int> dataLakeIds)
    {
      return ExecuteDbOperation(context => {
        var records = context.DataLakeOrganizacion.Where(
              c => c.IdVista == idVista &&
                   c.IdOrganizacion == idOrganizacion &&
                   dataLakeIds.Contains(c.IdDataLake)).ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdDataLakeOrganizacion).ToList();

        var deletedOrganizacionFullTextRecords = context.OrganizacionFullText.Where(o => deletedRecordIds.Contains(o.IdDataLakeOrganizacion)).ToList();
        context.OrganizacionFullText.RemoveRange(deletedOrganizacionFullTextRecords);
        context.SaveChanges();

        context.DataLakeOrganizacion.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }

    
    public bool DeleteByExcludingVistaIds(List<string> idsVista, string idOrganizacion, List<int> dataLakeIds, int idDataLakeOrganizacion)
    {
      return ExecuteDbOperation(context => {
        var records = context.DataLakeOrganizacion.AsNoTracking()
          .Where(c => c.IdOrganizacion == idOrganizacion && !idsVista.Contains(c.IdVista) && dataLakeIds.Contains(c.IdDataLake) && c.IdDataLakeOrganizacion != idDataLakeOrganizacion)
          .ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdDataLakeOrganizacion).ToList();

        var deletedOrganizacionFullTextRecords = context.OrganizacionFullText.Where(o => deletedRecordIds.Contains(o.IdDataLakeOrganizacion)).ToList();
        context.OrganizacionFullText.RemoveRange(deletedOrganizacionFullTextRecords);
        context.SaveChanges();

        context.DataLakeOrganizacion.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }
  }
}