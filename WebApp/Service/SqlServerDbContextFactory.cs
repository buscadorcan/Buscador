using Microsoft.EntityFrameworkCore;
using WebApp.Service.IService;
using WebApp.Service;

namespace WebApp.Service
{
  public class SqlServerDbContextFactory(DbContextOptions<SqlServerDbContext> options) : ISqlServerDbContextFactory
  {
    private readonly DbContextOptions<SqlServerDbContext> _options = options;

    /// <summary>
    /// Crea y devuelve una instancia de SqlServerDbContext configurada con las opciones proporcionadas.
    /// </summary>
    /// <returns>Una instancia de SqlServerDbContext configurada.</returns>
    public SqlServerDbContext CreateDbContext()
    {
      var context = new SqlServerDbContext(_options);
        
      // Desactivar el seguimiento de consultas
      context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

      // Deshabilitar la carga perezosa
      context.ChangeTracker.LazyLoadingEnabled = false;

      return context;
    }
  }
}
