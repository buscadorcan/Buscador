using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class OrganizacionDataRepository : BaseRepository, IOrganizacionDataRepository
  {
      public OrganizacionDataRepository(
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
      ) : base(sqlServerDbContextFactory, logger)
      {
      }

    public OrganizacionData? Create(OrganizacionData data)
    {
      data.IdOrganizacionData = 0;

      try {
        return ExecuteDbOperation(context => {
            context.OrganizacionData.Add(data);
            context.SaveChanges();
            return data;
        });
      } catch (Exception e) {
        Console.WriteLine(e);
        return null;
      }
    }

    public OrganizacionData? FindById(int Id)
    {
      return ExecuteDbOperation(context => context.OrganizacionData.AsNoTracking().FirstOrDefault(u => u.IdOrganizacionData == Id));
    }

    public ICollection<OrganizacionData> FindAll()
    {
      return ExecuteDbOperation(context => context.OrganizacionData.AsNoTracking().OrderBy(c => c.IdOrganizacionData).ToList());
    }

    public bool Update(OrganizacionData newRecord)
    {
      return ExecuteDbOperation(context => {
          var currentRecord = MergeEntityProperties(context, newRecord, u => u.IdOrganizacionData == newRecord.IdOrganizacionData);
          context.OrganizacionData.Update(currentRecord);
          return context.SaveChanges() >= 0;
      });
    }

    public int GetLastId()
    {
      return ExecuteDbOperation(context => context.OrganizacionData.AsNoTracking().Max(c => c.IdOrganizacionData));
    }

    public bool DeleteOldRecords(int IdHomologacionEsquema, int IdConexion)
    {
      return ExecuteDbOperation(context => {
        var records = context.OrganizacionData.Where(c => c.IdHomologacionEsquema == IdHomologacionEsquema && c.IdConexion == IdConexion).ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdOrganizacionData).ToList();

        var deletedOrganizacionFullTextRecords = context.OrganizacionFullText.Where(o => deletedRecordIds.Contains(o.IdOrganizacionData)).ToList();
        context.OrganizacionFullText.RemoveRange(deletedOrganizacionFullTextRecords);
        context.SaveChanges();

        context.OrganizacionData.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }

    public bool DeleteOnaRecords(int IdConexion)
    {
      return ExecuteDbOperation(context => {
        var records = context.OrganizacionData.Where(c => c.IdConexion == IdConexion).ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdOrganizacionData).ToList();

        var deletedOrganizacionFullTextRecords = context.OrganizacionFullText.Where(o => deletedRecordIds.Contains(o.IdOrganizacionData)).ToList();
        context.OrganizacionFullText.RemoveRange(deletedOrganizacionFullTextRecords);
        context.SaveChanges();

        context.OrganizacionData.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }

    public bool DeleteOldRecord(string idVista, string idOrganizacion, int IdConexion, int idHomologacionEsquema)
    {
      return ExecuteDbOperation(context => {
        var records = context.OrganizacionData.Where(
              c => c.IdVista == idVista &&
                   c.IdOrganizacion == idOrganizacion &&
                   c.IdConexion == IdConexion &&
                   c.IdHomologacionEsquema == idHomologacionEsquema
              ).ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdOrganizacionData).ToList();

        var deletedOrganizacionFullTextRecords = context.OrganizacionFullText.Where(o => deletedRecordIds.Contains(o.IdOrganizacionData)).ToList();
        context.OrganizacionFullText.RemoveRange(deletedOrganizacionFullTextRecords);
        context.SaveChanges();

        context.OrganizacionData.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }

    public bool DeleteByExcludingVistaIds(List<string> idsVista, string idOrganizacion, int idConexion, int idOrganizacionData)
    {
      return ExecuteDbOperation(context => {
        var records = context.OrganizacionData.AsNoTracking()
          .Where(c => c.IdOrganizacion == idOrganizacion &&
                      !idsVista.Contains(c.IdVista) &&
                      c.IdOrganizacionData != idOrganizacionData &&
                      c.IdConexion == idConexion)
          .ToList();
        List<int> deletedRecordIds = records.Select(r => r.IdOrganizacionData).ToList();

        var deletedOrganizacionFullTextRecords = context.OrganizacionFullText.Where(o => deletedRecordIds.Contains(o.IdOrganizacionData)).ToList();
        context.OrganizacionFullText.RemoveRange(deletedOrganizacionFullTextRecords);
        context.SaveChanges();

        context.OrganizacionData.RemoveRange(records);
        context.SaveChanges();

        return true;
      });
    }
  }
}