using System.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class LogMigracionRepository : BaseRepository, ILogMigracionRepository
    {
        public LogMigracionRepository(
          ILogger<LogMigracionRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
        }
        public LogMigracion Create(LogMigracion data)
        {
            return ExecuteDbOperation(context =>
            {
                context.LogMigracion.Add(data);
                context.SaveChanges();
                return data;
            });
        }
        public async Task<LogMigracion> CreateAsync(LogMigracion data)
        {
            return await ExecuteDbOperation(async context =>
            {
                context.LogMigracion.Add(data);
                await context.SaveChangesAsync();
                return data;
            });
        }

        public LogMigracion? FindById(int id)
        {
            return ExecuteDbOperation(context => context.LogMigracion.AsNoTracking().FirstOrDefault(u => u.IdLogMigracion == id));
        }
        public List<LogMigracion> FindAll()
        {
            return ExecuteDbOperation(context => context.LogMigracion.AsNoTracking().OrderByDescending(c => c.Fecha).ToList());
        }
        public bool Update(LogMigracion newRecord)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdLogMigracion == newRecord.IdLogMigracion);

                context.LogMigracion.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
        public async Task<bool> UpdateAsync(LogMigracion newRecord)
        {
            return await ExecuteDbOperation(async context =>
            {
                // Busca la entidad existente y combina las propiedades con el nuevo registro
                var existingRecord = MergeEntityProperties(context, newRecord, u => u.IdLogMigracion == newRecord.IdLogMigracion);

                if (existingRecord == null)
                {
                    // Si no existe, no se puede actualizar
                    return false;
                }

                // Marca la entidad como actualizada en el contexto
                context.LogMigracion.Update(existingRecord);

                // Guarda los cambios de forma asíncrona
                var rowsAffected = await context.SaveChangesAsync();
                return rowsAffected > 0;
            });
        }

        public LogMigracionDetalle CreateDetalle(LogMigracionDetalle data)
        {
            return ExecuteDbOperation(context =>
            {
                context.LogMigracionDetalle.Add(data);
                context.SaveChanges();
                return data;
            });
        }
        public async Task<LogMigracionDetalle> CreateDetalleAsync(LogMigracionDetalle data)
        {
            return await ExecuteDbOperation(async context =>
            {
                context.LogMigracionDetalle.Add(data);
                await context.SaveChangesAsync();
                return data;
            });
        }

        public List<LogMigracionDetalle> FindAllDetalle()
        {
            return ExecuteDbOperation(context => context.LogMigracionDetalle.AsNoTracking().OrderByDescending(c => c.Fecha).ToList());
        }
        public List<LogMigracionDetalle> FindDetalleById(int id)
        {
            return ExecuteDbOperation(context => context.LogMigracionDetalle.AsNoTracking().Where(u => u.IdLogMigracion == id).ToList());
        }
        public bool UpdateDetalle(LogMigracionDetalle newRecord)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdLogMigracionDetalle == newRecord.IdLogMigracionDetalle);

                context.LogMigracionDetalle.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
    }
}