using System.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccess.Models;
using SharedApp.Services;

namespace DataAccess.Repositories
{
  public class MigracionExcelRepository : BaseRepository, IMigracionExcelRepository
  {
    private readonly IJwtService _jwtService;
    public MigracionExcelRepository(
      IJwtService jwtService,
      ILogger<MigracionExcelRepository> logger,
      ISqlServerDbContextFactory sqlServerDbContextFactory
    ) : base(sqlServerDbContextFactory, logger)
    {
      _jwtService = jwtService;
    }
        //public MigracionExcel Create(MigracionExcel data)
        //{
        //  data.FechaCreacion = DateTime.Now;
        //  data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

        //  return ExecuteDbOperation(context => {
        //    context.MigracionExcel.Add(data);
        //    context.SaveChanges();
        //    return data;
        //  });
        //}
        public LogMigracion Create(LogMigracion data)
        {
            return ExecuteDbOperation(context => {
                context.LogMigracion.Add(data);
                context.SaveChanges();
                return data;
            });
        }
        public async Task<LogMigracion> CreateAsync(LogMigracion data)
        {
            try
            {
                return await ExecuteDbOperation(async context =>
                {
                    context.LogMigracion.Add(data);
                    await context.SaveChangesAsync();
                    return data;
                });
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public MigracionExcel? FindById(int id)
    {
      return ExecuteDbOperation(context => context.MigracionExcel.AsNoTracking().FirstOrDefault(u => u.IdMigracionExcel == id));
    }
    public List<MigracionExcel> FindAll()
    {
      return ExecuteDbOperation(context => context.MigracionExcel.AsNoTracking().OrderByDescending(c => c.FechaCreacion).ToList());
    }
        //public bool Update(MigracionExcel newRecord)
        //{
        //  return ExecuteDbOperation(context => {
        //    var _exits = MergeEntityProperties(context, newRecord, u => u.IdMigracionExcel == newRecord.IdMigracionExcel);

        //    context.MigracionExcel.Update(_exits);
        //    return context.SaveChanges() >= 0;
        //  });
        //}
     public bool Update(LogMigracion newRecord)
     {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdLogMigracion == newRecord.IdLogMigracion);

                context.LogMigracion.Update(_exits);
                return context.SaveChanges() >= 0;
            });
            //return true;
     }
        public async Task<bool> UpdateAsync(LogMigracion newRecord)
        {
            return await ExecuteDbOperation(async context =>
            {
                // Encuentra la entidad existente y mezcla las propiedades
                var existingRecord = MergeEntityProperties(context, newRecord, u => u.IdLogMigracion == newRecord.IdLogMigracion);

                if (existingRecord == null)
                {
                    // No existe el registro, no se puede actualizar
                    return false;
                }

                // Actualiza la entidad en el contexto
                context.LogMigracion.Update(existingRecord);

                // Guarda los cambios de forma asíncrona
                var rowsAffected = await context.SaveChangesAsync();
                return rowsAffected > 0;
            });
        }

    }
}
