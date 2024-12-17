namespace WebApp.Service.IService
{
  /// <summary>
  /// Define el contrato para un servicio que maneja la importación de datos desde archivos.
  /// </summary>
  public interface IImportador
  {
    /// <summary>
    /// Importa los datos desde los archivos especificados en las rutas proporcionadas.
    /// </summary>
    /// <param name="path">Un arreglo de rutas de archivos a importar.</param>
    /// <returns>Un valor booleano que indica si la importación fue exitosa.</returns>
    Boolean Importar(string[] path);
  }
}
