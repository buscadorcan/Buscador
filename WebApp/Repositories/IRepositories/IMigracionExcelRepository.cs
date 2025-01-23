using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IMigracionExcelRepository
  {
    //bool Update(MigracionExcel data);
    bool Update(LogMigracion data);
        Task<bool> UpdateAsync(LogMigracion data);


        //MigracionExcel Create(MigracionExcel data);
        LogMigracion Create(LogMigracion data);

        Task<LogMigracion> CreateAsync(LogMigracion data);

        MigracionExcel? FindById(int Id);
    List<MigracionExcel> FindAll();
  }
}