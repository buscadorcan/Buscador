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
        public async Task<EsquemaData?> CreateAsync(EsquemaData data)
        {
            data.IdEsquemaData = 0;

            try
            {
                return await ExecuteDbOperation(async context =>
                {
                    context.EsquemaData.Add(data);
                    await context.SaveChangesAsync();
                    return data;
                });
            }
            catch (Exception e)
            {
                // Registro del error
                Console.WriteLine($"Error al crear EsquemaData: {e.Message}");
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

        //public async Task<bool> DeleteOldRecordsAsync(int IdEsquemaVista)
        //{
        //    return ExecuteDbOperation(context => {
        //        var records = context.EsquemaData.Where(c => c.IdEsquemaVista == IdEsquemaVista).ToList();
        //        List<int> deletedRecordIds = records.Select(r => r.IdEsquemaData).ToList();

        //        var deletedCanFullTextRecords = context.EsquemaFullText.Where(o => deletedRecordIds.Contains(o.IdEsquemaData)).ToList();
        //        context.EsquemaFullText.RemoveRange(deletedCanFullTextRecords);
        //        context.SaveChanges();

        //        context.EsquemaData.RemoveRange(records);
        //        context.SaveChanges();

        //        return true;
        //    });
        //}
        public async Task<bool> DeleteOldRecordsAsync(int IdEsquemaVista)
        {
            return await ExecuteDbOperation(async context =>
            {
                // Obtén los registros de EsquemaData relacionados con IdEsquemaVista
                var records = await context.EsquemaData
                    .Where(c => c.IdEsquemaVista == IdEsquemaVista)
                    .ToListAsync();

                if (records.Any())
                {
                    // Obtén las IDs de los registros que serán eliminados
                    var deletedRecordIds = records.Select(r => r.IdEsquemaData).ToList();

                    // Busca los registros relacionados en EsquemaFullText
                    var deletedCanFullTextRecords = await context.EsquemaFullText
                        .Where(o => deletedRecordIds.Contains(o.IdEsquemaData))
                        .ToListAsync();

                    // Elimina los registros relacionados en EsquemaFullText
                    if (deletedCanFullTextRecords.Any())
                    {
                        context.EsquemaFullText.RemoveRange(deletedCanFullTextRecords);
                    }

                    // Elimina los registros en EsquemaData
                    context.EsquemaData.RemoveRange(records);

                    // Guarda los cambios en una única operación para garantizar atomicidad
                    await context.SaveChangesAsync();
                }

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
