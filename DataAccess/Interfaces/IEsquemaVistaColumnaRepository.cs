using DataAccess.Models;

namespace DataAccess.Interfaces
{
    public interface IEsquemaVistaColumnaRepository
    {
        /// <summary>
        /// WebApp/Update: Actualiza un registro de EsquemaVistaColumna en la base de datos.
        /// Este método permite modificar la información de una columna de un esquema vista existente.
        /// </summary>
        /// <param name="data">Objeto EsquemaVistaColumna con la información actualizada.</param>
        /// <returns>
        /// Devuelve un valor booleano indicando si la actualización fue exitosa.
        /// </returns>
        bool Update(EsquemaVistaColumna data);

        /// <summary>
        /// WebApp/Create: Crea un nuevo registro de EsquemaVistaColumna en la base de datos.
        /// Este método permite registrar una nueva columna dentro de un esquema vista.
        /// </summary>
        /// <param name="data">Objeto EsquemaVistaColumna con la información a registrar.</param>
        /// <returns>
        /// Devuelve un valor booleano indicando si la creación fue exitosa.
        /// </returns>
        bool Create(EsquemaVistaColumna data);

        /// <summary>
        /// WebApp/FindById: Busca un registro de EsquemaVistaColumna en la base de datos por su identificador único.
        /// Este método permite recuperar información de una columna específica dentro de un esquema vista en función de su ID.
        /// </summary>
        /// <param name="Id">Identificador único de la columna dentro del esquema vista.</param>
        /// <returns>
        /// Devuelve un objeto EsquemaVistaColumna con la información de la columna encontrada.
        /// Si no se encuentra el registro, devuelve un valor nulo.
        /// </returns>
        EsquemaVistaColumna? FindById(int Id);

        /// <summary>
        /// WebApp/FindByIdEsquemaVista: Obtiene una lista de columnas de un esquema vista específico.
        /// Este método permite recuperar todas las columnas pertenecientes a un esquema vista.
        /// </summary>
        /// <param name="IdEsquemaVista">Identificador del esquema vista.</param>
        /// <returns>
        /// Devuelve una lista de objetos EsquemaVistaColumna con la información de las columnas encontradas.
        /// </returns>
        List<EsquemaVistaColumna> FindByIdEsquemaVista(int IdEsquemaVista);

        /// <summary>
        /// WebApp/FindByIdEsquemaVistaOna: Obtiene una lista de columnas de un esquema vista específico dentro de un ONA.
        /// Este método permite recuperar todas las columnas de un esquema vista en un Organismo Nacional de Acreditación (ONA) específico.
        /// </summary>
        /// <param name="IdEsquemaVista">Identificador del esquema vista.</param>
        /// <param name="IdOna">Identificador del ONA.</param>
        /// <returns>
        /// Devuelve una lista de objetos EsquemaVistaColumna con la información de las columnas encontradas.
        /// </returns>
        List<EsquemaVistaColumna> FindByIdEsquemaVistaOna(int IdEsquemaVista, int IdOna);

        /// <summary>
        /// WebApp/FindByIdEsquemaVistaOnaAsync: Obtiene de forma asíncrona una lista de columnas de un esquema vista específico dentro de un ONA.
        /// Este método permite recuperar de manera asíncrona todas las columnas de un esquema vista en un Organismo Nacional de Acreditación (ONA) específico.
        /// </summary>
        /// <param name="IdEsquemaVista">Identificador del esquema vista.</param>
        /// <param name="IdOna">Identificador del ONA.</param>
        /// <returns>
        /// Devuelve una tarea (`Task<List<EsquemaVistaColumna>>`) con la lista de objetos EsquemaVistaColumna de las columnas encontradas.
        /// </returns>
        Task<List<EsquemaVistaColumna>> FindByIdEsquemaVistaOnaAsync(int IdEsquemaVista, int IdOna);

        /// <summary>
        /// WebApp/FindAll: Obtiene la lista completa de registros de EsquemaVistaColumna almacenados en la base de datos.
        /// Este método permite recuperar todos los registros de columnas en esquemas vista almacenados en la base de datos.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos EsquemaVistaColumna con la información de todas las columnas registradas.
        /// </returns>
        List<EsquemaVistaColumna> FindAll();


    }
}