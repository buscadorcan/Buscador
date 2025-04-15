namespace Core.Interfaces
{
    public interface IImportador
    {
        /// <summary>
        /// Importa datos desde los archivos especificados en las rutas proporcionadas.
        /// </summary>
        /// <param name="path">Un array de strings que contiene las rutas de los archivos a importar.</param>
        /// <returns>
        /// Un valor booleano que indica si la importación se realizó con éxito.
        /// Devuelve <c>true</c> si la importación fue exitosa, de lo contrario <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si el parámetro <paramref name="path"/> es nulo o está vacío.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Se lanza si uno o más archivos especificados en <paramref name="path"/> no existen.
        /// </exception>
        /// <exception cref="IOException">
        /// Se lanza si ocurre un error de entrada/salida durante la lectura de los archivos.
        /// </exception>
        /// <remarks>
        /// Este método se encarga de procesar los archivos en las rutas proporcionadas,
        /// validando su existencia y manejando posibles errores de importación.
        /// </remarks>
        bool Importar(string[] path);

    }
}