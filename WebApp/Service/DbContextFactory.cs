using Microsoft.EntityFrameworkCore;
using SharedApp.Data;
using WebApp.Service.IService;

namespace WebApp.Service
{
  public class DbContextFactory : IDbContextFactory
  {
    /// <summary>
    /// Crea una instancia de DbContext según el tipo de base de datos y la cadena de conexión proporcionados.
    /// </summary>
    /// <param name="connectionString">La cadena de conexión para la base de datos.</param>
    /// <param name="databaseType">El tipo de base de datos para determinar el proveedor de la base de datos.</param>
    /// <returns>Una instancia de DbContext configurada para el tipo de base de datos especificado.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Se lanza cuando el tipo de base de datos no es reconocido.</exception>
    public DbContext CreateDbContext(string connectionString, DatabaseType databaseType)
    {
      var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

      switch (databaseType)
      {
        case DatabaseType.MSSQL:
          optionsBuilder.UseSqlServer(connectionString);
          break;
        case DatabaseType.MYSQL:
          optionsBuilder.UseMySQL(connectionString);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
      }

      return new DynamicDbContext(optionsBuilder.Options);
    }
  }
}
