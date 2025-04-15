using DataAccess.Models;

namespace DataAccess.Interfaces
{
  public interface IConectionStringBuilderService
  {
        /// <summary>
        /// Construye una cadena de conexión a la base de datos en base a los datos de una conexión ONA proporcionada.
        /// </summary>
        /// <param name="conexion">Objeto <see cref="ONAConexion"/> que contiene los parámetros de conexión como servidor, base de datos, usuario y contraseña.</param>
        /// <returns>
        /// Devuelve un <see cref="string"/> con la cadena de conexión generada.
        /// </returns>
        /// <exception cref="ArgumentNullException">Lanzado si el objeto <paramref name="conexion"/> es nulo.</exception>
        /// <exception cref="InvalidOperationException">Lanzado si los datos de conexión son inválidos o incompletos.</exception>
        string BuildConnectionString(ONAConexion conexion);

    }
}
