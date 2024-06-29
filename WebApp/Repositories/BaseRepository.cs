using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApp.Service;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public abstract class BaseRepository
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory _dbContextFactory;

        protected BaseRepository(IDbContextFactory dbContextFactory, ILogger logger)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
        }

        protected TResult ExecuteDbOperation<TResult>(Func<SqlServerDbContext, TResult> operation)
        {
            try
            {
                using (var context = _dbContextFactory.CreateDbContext())
                {
                    return operation(context);
                }
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
                using (var context = _dbContextFactory.CreateDbContext())
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