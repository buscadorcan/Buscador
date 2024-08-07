using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class CanDataSetRepository : BaseRepository, ICanDataSetRepository
  {
      public CanDataSetRepository(
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
      ) : base(sqlServerDbContextFactory, logger)
      {
      }

    public CanDataSet? Create(CanDataSet data)
    {
      data.IdCanDataSet = 0;

      try {
        return ExecuteDbOperation(context => {
            context.CanDataSet.Add(data);
            context.SaveChanges();
            return data;
        });
      } catch (Exception e) {
        Console.WriteLine(e);
        return null;
      }
    }

    public CanDataSet? FindById(int Id)
    {
      return ExecuteDbOperation(context => context.CanDataSet.AsNoTracking().FirstOrDefault(u => u.IdCanDataSet == Id));
    }

    public ICollection<CanDataSet> FindAll()
    {
      return ExecuteDbOperation(context => context.CanDataSet.AsNoTracking().OrderBy(c => c.IdCanDataSet).ToList());
    }

    public bool Update(CanDataSet newRecord)
    {
      return ExecuteDbOperation(context => {
          var currentRecord = MergeEntityProperties(context, newRecord, u => u.IdCanDataSet == newRecord.IdCanDataSet);
          context.CanDataSet.Update(currentRecord);
          return context.SaveChanges() >= 0;
      });
    }

    public int GetLastId()
    {
      return ExecuteDbOperation(context => context.CanDataSet.AsNoTracking().Max(c => c.IdCanDataSet));
    }

    public bool DeleteOldRecords(int IdHomologacionEsquema, int IdConexion)
    {
      return ExecuteDbOperation(context => {
        var records = context.CanDataSet.Where(c => c.IdHomologacionEsquema == IdHomologacionEsquema && c.IdConexion == IdConexion).ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdCanDataSet).ToList();

        var deletedCanFullTextRecords = context.CanFullText.Where(o => deletedRecordIds.Contains(o.IdCanDataSet)).ToList();
        context.CanFullText.RemoveRange(deletedCanFullTextRecords);
        context.SaveChanges();

        context.CanDataSet.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }

    public bool DeleteOnaRecords(int IdConexion)
    {
      return ExecuteDbOperation(context => {
        var records = context.CanDataSet.Where(c => c.IdConexion == IdConexion).ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdCanDataSet).ToList();

        var deletedCanFullTextRecords = context.CanFullText.Where(o => deletedRecordIds.Contains(o.IdCanDataSet)).ToList();
        context.CanFullText.RemoveRange(deletedCanFullTextRecords);
        context.SaveChanges();

        context.CanDataSet.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }

    public bool DeleteOldRecord(string idVista, string idEnte, int IdConexion, int idHomologacionEsquema)
    {
      return ExecuteDbOperation(context => {
        var records = context.CanDataSet.Where(
              c => c.IdVista == idVista &&
                   c.IdEnte == idEnte &&
                   c.IdConexion == IdConexion &&
                   c.IdHomologacionEsquema == idHomologacionEsquema
              ).ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdCanDataSet).ToList();

        var deletedCanFullTextRecords = context.CanFullText.Where(o => deletedRecordIds.Contains(o.IdCanDataSet)).ToList();
        context.CanFullText.RemoveRange(deletedCanFullTextRecords);
        context.SaveChanges();

        context.CanDataSet.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }

    public bool DeleteByExcludingVistaIds(List<string> idsVista, string idEnte, int idConexion, int idCanDataSet)
    {
      return ExecuteDbOperation(context => {
        var records = context.CanDataSet.AsNoTracking()
          .Where(c => c.IdEnte == idEnte &&
                      !idsVista.Contains(c.IdVista) &&
                      c.IdCanDataSet != idCanDataSet &&
                      c.IdConexion == idConexion)
          .ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdCanDataSet).ToList();

        var deletedCanFullTextRecords = context.CanFullText.Where(o => deletedRecordIds.Contains(o.IdCanDataSet)).ToList();
        context.CanFullText.RemoveRange(deletedCanFullTextRecords);
        context.SaveChanges();

        context.CanDataSet.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }
  }
}