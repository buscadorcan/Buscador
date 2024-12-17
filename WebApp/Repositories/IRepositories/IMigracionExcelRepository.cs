using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IMigracionExcelRepository
  {
    bool Update(MigracionExcel data);
    MigracionExcel Create(MigracionExcel data);
    MigracionExcel? FindById(int Id);
    List<MigracionExcel> FindAll();
  }
}