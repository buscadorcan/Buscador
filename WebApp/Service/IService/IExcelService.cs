
using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IExcelService
    {
        //Boolean ImportarExcel(string path, MigracionExcel migracion);
        Boolean ImportarExcel(string path, LogMigracion migracion, int idOna);
    }
}