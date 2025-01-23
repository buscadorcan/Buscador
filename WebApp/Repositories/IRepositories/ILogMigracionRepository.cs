using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface ILogMigracionRepository
  {
    bool Update(LogMigracion data);
    Task<bool> UpdateAsync(LogMigracion data);
    LogMigracion Create(LogMigracion data);
    Task<LogMigracion> CreateAsync(LogMigracion data);
    LogMigracion? FindById(int Id);
    List<LogMigracion> FindAll();
    bool UpdateDetalle(LogMigracionDetalle data);
    LogMigracionDetalle CreateDetalle(LogMigracionDetalle data);
        Task<LogMigracionDetalle> CreateDetalleAsync(LogMigracionDetalle data);

        List<LogMigracionDetalle> FindAllDetalle();
    List<LogMigracionDetalle> FindDetalleById(int Id);
  }
}
