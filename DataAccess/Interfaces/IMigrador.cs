using DataAccess.Models;

namespace DataAccess.Interfaces
{
    public interface IMigrador
    {
        /// <summary>
        /// Realiza la migración de datos de manera asíncrona utilizando la conexión ONA especificada.
        /// </summary>
        /// <param name="conexion">Objeto <see cref="ONAConexion"/> que contiene los detalles de la conexión ONA.</param>
        /// <returns>
        /// Una tarea que representa la operación asincrónica, con un valor booleano que indica si la migración fue exitosa.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si el parámetro <paramref name="conexion"/> es nulo.
        /// </exception>
        /// <exception cref="DatabaseException">
        /// Se lanza si ocurre un error durante la migración de datos en la base de datos.
        /// </exception>
        /// <remarks>
        /// Este método establece una conexión con la base de datos ONA y ejecuta la lógica de migración de datos.
        /// La migración puede incluir transferencia de datos, validaciones y transformaciones.
        /// </remarks>
        Task<bool> MigrarAsync(ONAConexion conexion);

    }
}
