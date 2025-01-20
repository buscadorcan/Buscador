using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IMigracionExcelRepository
  {
    //bool Update(MigracionExcel data);
    bool Update(LogMigracion data);

    //MigracionExcel Create(MigracionExcel data);
    LogMigracion Create(LogMigracion data);
    MigracionExcel? FindById(int Id);
    List<MigracionExcel> FindAll();
  }
}