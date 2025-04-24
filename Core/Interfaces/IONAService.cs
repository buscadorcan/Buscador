using DataAccess.Models;

namespace Core.Interfaces
{
    public interface IONAService
    {
        /// <summary>
        /// Actualiza un registro de <see cref="ONA"/> en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="ONA"/> con los datos actualizados.</param>
        /// <returns>Devuelve <c>true</c> si la actualización fue exitosa, de lo contrario <c>false</c>.</returns>
        bool Update(ONA data);

        /// <summary>
        /// Crea un nuevo registro de <see cref="ONA"/> en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="ONA"/> con los datos a insertar.</param>
        /// <returns>Devuelve <c>true</c> si la operación fue exitosa, de lo contrario <c>false</c>.</returns>
        bool Create(ONA data);

        /// <summary>
        /// Busca un registro de <see cref="ONA"/> en la base de datos por su identificador único.
        /// </summary>
        /// <param name="Id">Identificador único del ONA.</param>
        /// <returns>Devuelve un objeto <see cref="ONA"/> si se encuentra, de lo contrario <c>null</c>.</returns>
        ONA? FindById(int Id);

        /// <summary>
        /// Busca un registro de <see cref="ONA"/> en la base de datos por sus siglas.
        /// </summary>
        /// <param name="siglas">Siglas del ONA a buscar.</param>
        /// <returns>Devuelve un objeto <see cref="ONA"/> si se encuentra, de lo contrario <c>null</c>.</returns>
        ONA? FindBySiglas(string siglas);

        /// <summary>
        /// Obtiene la lista completa de registros de <see cref="ONA"/> almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve una lista de objetos <see cref="ONA"/>.</returns>
        List<ONA> FindAll();

        /// <summary>
        /// Obtiene la lista de países relacionados con las ONAs.
        /// </summary>
        /// <returns>Devuelve una lista de objetos <see cref="VwPais"/> con la información de los países.</returns>
        List<VwPais> FindAllPaises();

        /// <summary>
        /// Busca de forma asíncrona un registro de <see cref="ONA"/> en la base de datos por su identificador único.
        /// </summary>
        /// <param name="Id">Identificador único del ONA.</param>
        /// <returns>Devuelve un objeto <see cref="ONA"/> si se encuentra, de lo contrario <c>null</c>.</returns>
        Task<ONA?> FindByIdAsync(int Id);

        /// <summary>
        /// Obtiene de forma asíncrona una lista de ONAs en base a un identificador específico.
        /// </summary>
        /// <param name="idOna">Identificador del ONA para filtrar la búsqueda.</param>
        /// <returns>Devuelve una lista de objetos <see cref="ONA"/>.</returns>
        List<ONA> GetListByONAsAsync(int idOna);
    }
}
