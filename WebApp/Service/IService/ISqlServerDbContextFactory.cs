namespace WebApp.Service.IService
{
  /// <summary>
  /// Define un contrato para la creación de instancias de <see cref="SqlServerDbContext"/>.
  /// </summary>
  public interface ISqlServerDbContextFactory
  {
    /// <summary>
    /// Crea una nueva instancia de <see cref="SqlServerDbContext"/>.
    /// </summary>
    /// <returns>Una nueva instancia de <see cref="SqlServerDbContext"/>.</returns>
    SqlServerDbContext CreateDbContext();
  }
}
