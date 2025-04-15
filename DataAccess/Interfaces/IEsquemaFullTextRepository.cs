using DataAccess.Models;

namespace DataAccess.Interfaces
{
  public interface IEsquemaFullTextRepository
  {
        /// <summary>
        /// WebApp/Create: Crea un nuevo registro de EsquemaFullText en la base de datos.
        /// Este método permite insertar un nuevo registro de EsquemaFullText con la información proporcionada.
        /// </summary>
        /// <param name="data">Objeto EsquemaFullText con los datos a registrar.</param>
        /// <returns>
        /// Devuelve un objeto EsquemaFullText con la información del registro creado.
        /// </returns>
        EsquemaFullText Create(EsquemaFullText data);

        /// <summary>
        /// WebApp/CreateAsync: Crea un nuevo registro de EsquemaFullText en la base de datos de forma asíncrona.
        /// Este método permite insertar un nuevo registro de EsquemaFullText de manera asíncrona.
        /// </summary>
        /// <param name="data">Objeto EsquemaFullText con los datos a registrar.</param>
        /// <returns>
        /// Devuelve una tarea (`Task<EsquemaFullText>`) con el objeto EsquemaFullText del registro creado.
        /// </returns>
        Task<EsquemaFullText> CreateAsync(EsquemaFullText data);

        /// <summary>
        /// WebApp/FindById: Busca un registro de EsquemaFullText en la base de datos por su identificador único.
        /// Este método permite recuperar un registro específico de EsquemaFullText en función de su ID.
        /// </summary>
        /// <param name="Id">Identificador único del registro a buscar.</param>
        /// <returns>
        /// Devuelve un objeto EsquemaFullText con la información del registro encontrado.
        /// Si no se encuentra el registro, devuelve un valor nulo.
        /// </returns>
        EsquemaFullText? FindById(int Id);


    }
}
