using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApp.Service;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public abstract class BaseRepository
  {
    private readonly ILogger _logger;
    private readonly ISqlServerDbContextFactory _sqlServerDbContextFactory;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="BaseRepository"/>.
    /// </summary>
    /// <param name="sqlServerDbContextFactory">Fábrica para crear instancias del contexto de base de datos <see cref="SqlServerDbContext"/>.</param>
    /// <param name="logger">Instancia del logger para registrar errores.</param>
    protected BaseRepository(ISqlServerDbContextFactory sqlServerDbContextFactory, ILogger logger)
    {
      _logger = logger;
      _sqlServerDbContextFactory = sqlServerDbContextFactory;
    }

    /// <summary>
    /// Ejecuta una operación de base de datos de manera sincrónica.
    /// </summary>
    /// <typeparam name="TResult">Tipo de resultado de la operación.</typeparam>
    /// <param name="operation">Función que ejecuta la operación sobre el contexto de base de datos.</param>
    /// <returns>El resultado de la operación.</returns>
    /// <exception cref="Exception">Lanza una excepción si la operación falla.</exception>
    protected TResult ExecuteDbOperation<TResult>(Func<SqlServerDbContext, TResult> operation)
    {
      try
      {
        using (var context = _sqlServerDbContextFactory.CreateDbContext())
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

    /// <summary>
    /// Ejecuta una operación de base de datos de manera asincrónica.
    /// </summary>
    /// <typeparam name="TResult">Tipo de resultado de la operación.</typeparam>
    /// <param name="operation">Función que ejecuta la operación asincrónica sobre el contexto de base de datos.</param>
    /// <returns>El resultado de la operación asincrónica.</returns>
    /// <exception cref="Exception">Lanza una excepción si la operación falla.</exception>
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

    /// <summary>
    /// Fusiona las propiedades de una entidad con una entidad existente en el contexto de base de datos.
    /// </summary>
    /// <typeparam name="TEntity">Tipo de la entidad.</typeparam>
    /// <param name="context">El contexto de base de datos donde se buscará la entidad existente.</param>
    /// <param name="entity">La entidad que contiene los nuevos valores a fusionar.</param>
    /// <param name="predicate">Expresión para buscar la entidad existente.</param>
    /// <returns>La entidad existente con los valores fusionados.</returns>
    /// <exception cref="Exception">Lanza una excepción si la entidad no se encuentra.</exception>
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
