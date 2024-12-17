using WebApp.Models;

namespace WebApp.Service.IService
{
  /// <summary>
  /// Define un contrato para un servicio de importación de datos desde archivos Excel.
  /// </summary>
  public interface IExcelService
  {
    /// <summary>
    /// Importa los datos de un archivo Excel a un proceso de migración específico.
    /// </summary>
    /// <param name="path">La ruta del archivo Excel a importar.</param>
    /// <param name="migracion">El objeto que representa el proceso de migración al que se asociarán los datos importados.</param>
    /// <returns>Un valor booleano que indica si la importación fue exitosa o no.</returns>
    Boolean ImportarExcel(string path, MigracionExcel migracion);
  }
}
