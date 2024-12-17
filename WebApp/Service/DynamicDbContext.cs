using Microsoft.EntityFrameworkCore;

namespace WebApp.Service
{
  public class DynamicDbContext : DbContext
  {
    /// <summary>
    /// Constructor que inicializa una nueva instancia de DynamicDbContext.
    /// </summary>
    /// <param name="options">Las opciones de configuración del DbContext.</param>
    public DynamicDbContext(DbContextOptions<DbContext> options) : base(options) { }
  }
}
