using DataAccess.Models;

namespace Core.Interfaces
{
    public interface IExcelService
    {
        /// <summary>
        /// Importa un archivo de Excel desde la ruta especificada y registra la migración en el sistema para un ONA determinado.
        /// </summary>
        /// <param name="path">Ruta del archivo de Excel a importar.</param>
        /// <param name="migracion">Registro de la migración asociado al proceso de importación.</param>
        /// <param name="idOna">Identificador del ONA al que pertenece la migración.</param>
        /// <returns>
        /// Devuelve un <see cref="Task{Boolean}"/> que representa el resultado de la importación:
        /// <c>true</c> si la importación fue exitosa, <c>false</c> si ocurrió un error.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si <paramref name="path"/> o <paramref name="migracion"/> son nulos o vacíos.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Se lanza si el archivo especificado en <paramref name="path"/> no existe.
        /// </exception>
        /// <exception cref="FormatException">
        /// Se lanza si el archivo de Excel no tiene el formato esperado.
        /// </exception>
        /// <exception cref="IOException">
        /// Se lanza si hay un error al leer el archivo, como problemas de acceso o corrupción del archivo.
        /// </exception>
        /// <exception cref="DbUpdateException">
        /// Se lanza si hay un error al registrar la migración en la base de datos.
        /// </exception>
        Task<bool> ImportarExcel(string path, LogMigracion migracion, int idOna);

    }
}