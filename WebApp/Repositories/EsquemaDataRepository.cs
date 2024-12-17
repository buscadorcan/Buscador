using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class EsquemaDataRepository : BaseRepository, IEsquemaDataRepository
  {
    public EsquemaDataRepository(
      ILogger<UsuarioRepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
    }
    public EsquemaData? Create(EsquemaData data)
    {
      data.IdEsquemaData = 0;

      try {
        return ExecuteDbOperation(context => {
          context.EsquemaData.Add(data);
          context.SaveChanges();
          return data;
        });
      } catch (Exception e) {
        Console.WriteLine(e);
        return null;
      }
    }
    public EsquemaData? FindById(int Id)
    {
      return ExecuteDbOperation(context => context.EsquemaData.AsNoTracking().FirstOrDefault(u => u.IdEsquemaData == Id));
    }
    public ICollection<EsquemaData> FindAll()
    {
      return ExecuteDbOperation(context => context.EsquemaData.AsNoTracking().OrderBy(c => c.IdEsquemaData).ToList());
    }
    public bool Update(EsquemaData newRecord)
    {
      return ExecuteDbOperation(context => {
          var currentRecord = MergeEntityProperties(context, newRecord, u => u.IdEsquemaData == newRecord.IdEsquemaData);
          context.EsquemaData.Update(currentRecord);
          return context.SaveChanges() >= 0;
      });
    }
    public int GetLastId()
    {
      return ExecuteDbOperation(context => context.EsquemaData.AsNoTracking().Max(c => c.IdEsquemaData));
    }
    public bool DeleteOldRecords(int IdEsquemaVista)
    {
      return ExecuteDbOperation(context => {
        var records = context.EsquemaData.Where(c => c.IdEsquemaVista == IdEsquemaVista).ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdEsquemaData).ToList();

        var deletedCanFullTextRecords = context.EsquemaFullText.Where(o => deletedRecordIds.Contains(o.IdEsquemaData)).ToList();
        context.EsquemaFullText.RemoveRange(deletedCanFullTextRecords);
        context.SaveChanges();

        context.EsquemaData.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }
    public bool DeleteOnaRecords(int IdConexion)
    {
      return ExecuteDbOperation(context => {
        // var records = context.EsquemaData.Where(c => c. == IdConexion).ToList();
        var records = new List<EsquemaData>();
        List<int> deletedRecordIds = records.Select(r => r.IdEsquemaData).ToList();

        var deletedCanFullTextRecords = context.EsquemaFullText.Where(o => deletedRecordIds.Contains(o.IdEsquemaData)).ToList();
        context.EsquemaFullText.RemoveRange(deletedCanFullTextRecords);
        context.SaveChanges();

        context.EsquemaData.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }
    public bool DeleteOldRecord(string idVista, string idEnte, int IdConexion, int idHomologacionEsquema)
    {
      return ExecuteDbOperation(context => {
        // var records = context.EsquemaData.Where(
        //       c => c.IdVista == idVista &&
        //            c.IdEnte == idEnte &&
        //            c.IdConexion == IdConexion &&
        //            c.IdHomologacionEsquema == idHomologacionEsquema
        //       ).ToList();
        var records = new List<EsquemaData>();
        List<int> deletedRecordIds = records.Select(r => r.IdEsquemaData).ToList();

        var deletedCanFullTextRecords = context.EsquemaFullText.Where(o => deletedRecordIds.Contains(o.IdEsquemaData)).ToList();
        context.EsquemaFullText.RemoveRange(deletedCanFullTextRecords);
        context.SaveChanges();

        context.EsquemaData.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }
    public bool DeleteByExcludingVistaIds(List<string> idsVista, string idEnte, int idConexion, int idEsquemaData)
    {
      return ExecuteDbOperation(context => {
        // var records = context.EsquemaData.AsNoTracking()
        //   .Where(c => c.IdEnte == idEnte &&
        //               !idsVista.Contains(c.IdVista) &&
        //               c.IdEsquemaData != idEsquemaData &&
        //               c.IdConexion == idConexion)
        //   .ToList();
        var records = new List<EsquemaData>();
        List<int> deletedRecordIds = records.Select(r => r.IdEsquemaData).ToList();

        var deletedCanFullTextRecords = context.EsquemaFullText.Where(o => deletedRecordIds.Contains(o.IdEsquemaData)).ToList();
        context.EsquemaFullText.RemoveRange(deletedCanFullTextRecords);
        context.SaveChanges();

        context.EsquemaData.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }
  }
}
