using DataAccess.Data;

namespace DataAccess.Interfaces
{
  
  public interface ISqlServerDbContextFactory
  {
        /// <summary>
        /// Crea y devuelve una nueva instancia de <see cref="SqlServerDbContext"/> 
        /// para la gestión de la base de datos en SQL Server.
        /// </summary>
        /// <returns>Devuelve una instancia de <see cref="SqlServerDbContext"/> lista para ser utilizada.</returns>
        SqlServerDbContext CreateDbContext();
  }
}
