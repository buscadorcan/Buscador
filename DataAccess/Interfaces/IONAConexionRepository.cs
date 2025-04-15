using DataAccess.Models;

namespace DataAccess.Interfaces
{
    public interface IONAConexionRepository
    {
        /// <summary>
        /// Actualiza un registro de <see cref="ONAConexion"/> en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="ONAConexion"/> con los datos actualizados.</param>
        /// <returns>Devuelve <c>true</c> si la actualización fue exitosa, de lo contrario <c>false</c>.</returns>
        bool Update(ONAConexion data);

        /// <summary>
        /// Crea un nuevo registro de <see cref="ONAConexion"/> en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="ONAConexion"/> con los datos a insertar.</param>
        /// <returns>Devuelve <c>true</c> si la operación fue exitosa, de lo contrario <c>false</c>.</returns>
        bool Create(ONAConexion data);

        /// <summary>
        /// Busca un registro de <see cref="ONAConexion"/> en la base de datos por su identificador único.
        /// </summary>
        /// <param name="Id">Identificador único del registro.</param>
        /// <returns>Devuelve un objeto <see cref="ONAConexion"/> si se encuentra, de lo contrario <c>null</c>.</returns>
        ONAConexion? FindById(int Id);

        /// <summary>
        /// Busca un registro de <see cref="ONAConexion"/> en la base de datos por el identificador de ONA.
        /// </summary>
        /// <param name="IdONA">Identificador del ONA asociado.</param>
        /// <returns>Devuelve un objeto <see cref="ONAConexion"/> si se encuentra, de lo contrario <c>null</c>.</returns>
        ONAConexion? FindByIdONA(int IdONA);

        /// <summary>
        /// Busca de forma asíncrona un registro de <see cref="ONAConexion"/> en la base de datos por el identificador de ONA.
        /// </summary>
        /// <param name="IdONA">Identificador del ONA asociado.</param>
        /// <returns>Devuelve un <see cref="Task{ONAConexion}"/> con el objeto encontrado o <c>null</c> si no existe.</returns>
        Task<ONAConexion?> FindByIdONAAsync(int IdONA);

        /// <summary>
        /// Obtiene la lista completa de registros de <see cref="ONAConexion"/> almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve una lista de objetos <see cref="ONAConexion"/>.</returns>
        List<ONAConexion> FindAll();

        /// <summary>
        /// Obtiene de forma asíncrona una lista de conexiones de <see cref="ONAConexion"/> por su identificador de ONA.
        /// </summary>
        /// <param name="IdONA">Identificador del ONA.</param>
        /// <returns>Devuelve una lista de objetos <see cref="ONAConexion"/> relacionados con el ONA especificado.</returns>
        List<ONAConexion> GetOnaConexionByOnaListAsync(int IdONA);

    }
}
