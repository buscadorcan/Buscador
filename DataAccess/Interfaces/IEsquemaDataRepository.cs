using DataAccess.Models;

namespace DataAccess.Interfaces
{
  public interface IEsquemaDataRepository
  {
        /// <summary>
        /// WebApp/Update: Actualiza un registro en la base de datos con los datos proporcionados.
        /// Este método permite modificar un registro existente en la base de datos con la información nueva ingresada.
        /// </summary>
        /// <param name="data">Objeto EsquemaData con los datos actualizados.</param>
        /// <returns>
        /// Devuelve un valor booleano indicando si la actualización fue exitosa o no.
        /// </returns>
        bool Update(EsquemaData data);

        /// <summary>
        /// WebApp/Create: Crea un nuevo registro en la base de datos.
        /// Este método permite insertar un nuevo registro en la base de datos con la información proporcionada.
        /// </summary>
        /// <param name="data">Objeto EsquemaData con los datos a registrar.</param>
        /// <returns>
        /// Devuelve un objeto EsquemaData con la información del registro creado.
        /// Si la creación falla, devuelve un valor nulo.
        /// </returns>
        EsquemaData? Create(EsquemaData data);

        /// <summary>
        /// WebApp/CreateAsync: Crea un nuevo registro en la base de datos de forma asíncrona.
        /// Este método permite insertar un nuevo registro en la base de datos de manera asíncrona.
        /// </summary>
        /// <param name="data">Objeto EsquemaData con los datos a registrar.</param>
        /// <returns>
        /// Devuelve una tarea (`Task<EsquemaData?>`) con el objeto EsquemaData del registro creado.
        /// Si la creación falla, devuelve un valor nulo.
        /// </returns>
        Task<EsquemaData?> CreateAsync(EsquemaData data);

        /// <summary>
        /// WebApp/FindById: Busca un registro en la base de datos por su identificador único.
        /// Este método permite recuperar un registro específico en función de su ID.
        /// </summary>
        /// <param name="Id">Identificador único del registro a buscar.</param>
        /// <returns>
        /// Devuelve un objeto EsquemaData con la información del registro encontrado.
        /// Si no se encuentra el registro, devuelve un valor nulo.
        /// </returns>
        EsquemaData? FindById(int Id);

        /// <summary>
        /// WebApp/FindAll: Obtiene la colección completa de registros almacenados en la base de datos.
        /// Este método permite recuperar todos los registros almacenados en la base de datos.
        /// </summary>
        /// <returns>
        /// Devuelve una colección de objetos EsquemaData con todos los registros existentes en la base de datos.
        /// </returns>
        ICollection<EsquemaData> FindAll();

        /// <summary>
        /// WebApp/GetLastId: Obtiene el último identificador registrado en la base de datos.
        /// Este método permite recuperar el ID más reciente de los registros almacenados en la base de datos.
        /// </summary>
        /// <returns>
        /// Devuelve un entero con el último identificador registrado.
        /// </returns>
        int GetLastId();


        /// <summary>
        /// WebApp/DeleteOldRecords: Elimina registros antiguos asociados a un ONA específico.
        /// Este método permite eliminar registros obsoletos de la base de datos que pertenecen a un Organismo Nacional de Acreditación (ONA).
        /// </summary>
        /// <param name="idONA">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve un valor booleano indicando si la eliminación fue exitosa.
        /// </returns>
        bool DeleteOldRecords(int idONA);

        /// <summary>
        /// WebApp/DeleteOldRecordsAsync: Elimina registros antiguos de un esquema de vista de manera asíncrona.
        /// Este método permite eliminar de forma asíncrona los registros obsoletos asociados a un esquema de vista.
        /// </summary>
        /// <param name="IdEsquemaVista">Identificador del esquema de vista cuyos registros antiguos deben eliminarse.</param>
        /// <returns>
        /// Devuelve una tarea (`Task<bool>`) indicando si la eliminación fue exitosa.
        /// </returns>
        Task<bool> DeleteOldRecordsAsync(int IdEsquemaVista);

        /// <summary>
        /// WebApp/DeleteOldRecord: Elimina un registro antiguo específico utilizando identificadores de vista, ente, conexión y esquema de homologación.
        /// Este método permite borrar un registro obsoleto específico en función de múltiples identificadores clave.
        /// </summary>
        /// <param name="idVista">Identificador de la vista a la que pertenece el registro.</param>
        /// <param name="idEnte">Identificador del ente asociado al registro.</param>
        /// <param name="idConexion">Identificador de la conexión utilizada para el registro.</param>
        /// <param name="idHomologacionEsquema">Identificador del esquema de homologación del registro.</param>
        /// <returns>
        /// Devuelve un valor booleano indicando si la eliminación del registro fue exitosa.
        /// </returns>
        bool DeleteOldRecord(string idVista, string idEnte, int idConexion, int idHomologacionEsquema);

        /// <summary>
        /// WebApp/DeleteByExcludingVistaIds: Elimina registros excluyendo aquellos con los identificadores de vista proporcionados.
        /// Este método permite eliminar registros antiguos pero preservando aquellos cuyos identificadores de vista se encuentran en la lista dada.
        /// </summary>
        /// <param name="idsVista">Lista de identificadores de vista que no deben eliminarse.</param>
        /// <param name="idEnte">Identificador del ente asociado a los registros.</param>
        /// <param name="idConexion">Identificador de la conexión utilizada.</param>
        /// <param name="idEsquemaData">Identificador del esquema de datos.</param>
        /// <returns>
        /// Devuelve un valor booleano indicando si la operación de eliminación fue exitosa.
        /// </returns>
        bool DeleteByExcludingVistaIds(List<string> idsVista, string idEnte, int idConexion, int idEsquemaData);

        /// <summary>
        /// WebApp/DeleteDataAntigua: Elimina datos antiguos de un ONA específico.
        /// Este método permite borrar datos obsoletos almacenados en la base de datos para un Organismo Nacional de Acreditación (ONA).
        /// </summary>
        /// <param name="idONA">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve un valor booleano indicando si la eliminación de los datos antiguos fue exitosa.
        /// </returns>
        bool DeleteDataAntigua(int idONA);


    }
}
