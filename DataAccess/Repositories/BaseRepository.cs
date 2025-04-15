using System.Data.SqlTypes;
using System.Reflection;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace DataAccess.Repositories
{
  public abstract class BaseRepository
  {
    public readonly ILogger _logger;
    private readonly ISqlServerDbContextFactory _sqlServerDbContextFactory;
    protected BaseRepository(ISqlServerDbContextFactory sqlServerDbContextFactory, ILogger logger)
    {
      _logger = logger;
      _sqlServerDbContextFactory = sqlServerDbContextFactory;
    }
        protected TResult ExecuteDbOperation<TResult>(Func<SqlServerDbContext, TResult> operation)
        {
            try
            {
                using (var context = _sqlServerDbContextFactory.CreateDbContext())
                {
                    return operation(context);
                }
            }
            catch (SqlNullValueException ex)
            {
                _logger.LogError(ex, "SQL Null value exception occurred");
                throw new Exception("A null value was encountered in the database operation", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing database operation");
                throw new Exception("Database operation failed", ex);
            }
        }
        protected async Task<TResult> ExecuteDbOperationAsync<TResult>(Func<SqlServerDbContext, Task<TResult>> operation)
    {
      try
      {
        using (var context = _sqlServerDbContextFactory.CreateDbContext())
        {
          return await operation(context);
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error executing database operation");
        throw new Exception("Database operation failed", ex);
      }
    }
    protected TEntity MergeEntityProperties<TEntity>(DbContext context, TEntity entity, Func<TEntity, bool> predicate) where TEntity : class
    {
      var existingEntity = context.Set<TEntity>().AsNoTracking().FirstOrDefault(predicate);
      if (existingEntity == null)
      {
        throw new Exception($"{typeof(TEntity).Name} not found");
      }

      PropertyInfo[] properties = typeof(TEntity).GetProperties();
      foreach (PropertyInfo property in properties)
      {
        var newValue = property.GetValue(entity);
        var oldValue = property.GetValue(existingEntity);

        if (newValue != null && !Equals(newValue, oldValue))
        {
          property.SetValue(existingEntity, newValue);
        }
      }
    
      return existingEntity;
    }
  }
}
