using DataAccess.Models;

namespace DataAccess.Interfaces
{
  public interface IUsuarioEndpointRepository
  {
        /// <summary>
        /// Obtiene la lista completa de registros de UsuarioEndpoint almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve una colección de objetos <see cref="UsuarioEndpoint"/>.</returns>
        ICollection<UsuarioEndpoint> FindAll();

        /// <summary>
        /// Busca un registro de UsuarioEndpoint en la base de datos por su identificador de endpoint.
        /// </summary>
        /// <param name="endpointId">El identificador único del endpoint.</param>
        /// <returns>Devuelve un objeto <see cref="UsuarioEndpoint"/> si se encuentra, de lo contrario, devuelve null.</returns>
        UsuarioEndpoint? FindByEndpointId(int endpointId);

        /// <summary>
        /// Crea un nuevo registro de UsuarioEndpoint en la base de datos.
        /// </summary>
        /// <param name="UsuarioEndpoint">Objeto de tipo <see cref="UsuarioEndpoint"/> que contiene los datos a almacenar.</param>
        /// <returns>Devuelve un valor booleano que indica si la operación fue exitosa.</returns>
        bool Create(UsuarioEndpoint UsuarioEndpoint);

    }
}
