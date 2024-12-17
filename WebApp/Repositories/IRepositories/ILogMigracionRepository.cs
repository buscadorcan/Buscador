using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface ILogMigracionRepository
  {
    bool Update(LogMigracion data);
    LogMigracion Create(LogMigracion data);
    LogMigracion? FindById(int Id);
    List<LogMigracion> FindAll();
    bool UpdateDetalle(LogMigracionDetalle data);
    LogMigracionDetalle CreateDetalle(LogMigracionDetalle data);
    List<LogMigracionDetalle> FindAllDetalle();
    List<LogMigracionDetalle> FindDetalleById(int Id);
  }
}
