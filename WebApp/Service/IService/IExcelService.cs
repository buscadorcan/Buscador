
using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IExcelService
    {
        //Boolean ImportarExcel(string path, MigracionExcel migracion);
        Task<Boolean> ImportarExcel(string path, LogMigracion migracion, int idOna);
        //Task <Boolean> ImportarExcelAsync(string path, LogMigracion migracion, int idOna);

    }
}