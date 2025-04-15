using SharedApp.Dtos;
using DataAccess.Models;

namespace DataAccess.Interfaces
{
    public interface IDynamicRepository
    {
        /// <summary>
        /// WebApp/GetProperties: Obtiene las propiedades de una tabla dentro de una vista específica.
        /// Este método permite recuperar la configuración y estructura de las columnas de una tabla en una vista dentro de un ONA.
        /// </summary>
        /// <param name="idONA">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <param name="viewName">Nombre de la vista en la que se encuentra la tabla.</param>
        /// <returns>
        /// Devuelve una lista de objetos PropiedadesTablaDto con las propiedades de la tabla dentro de la vista.
        /// </returns>
        List<PropiedadesTablaDto> GetProperties(int idONA, string viewName);

        /// <summary>
        /// WebApp/GetValueColumna: Obtiene los valores de una columna específica dentro de una vista.
        /// Este método permite recuperar los valores almacenados en una columna de una vista específica dentro de un ONA.
        /// </summary>
        /// <param name="idONA">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <param name="valueColumn">Nombre de la columna cuyos valores se desean obtener.</param>
        /// <param name="viewName">Nombre de la vista en la que se encuentra la columna.</param>
        /// <returns>
        /// Devuelve una lista de objetos PropiedadesTablaDto con los valores de la columna dentro de la vista.
        /// </returns>
        List<PropiedadesTablaDto> GetValueColumna(int idONA, string valueColumn, string viewName);

        /// <summary>
        /// WebApp/GetViewNames: Obtiene la lista de nombres de vistas disponibles en un ONA.
        /// Este método permite recuperar todas las vistas disponibles dentro de la base de datos de un ONA.
        /// </summary>
        /// <param name="idONA">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve una lista de nombres de vistas disponibles en el ONA.
        /// </returns>
        List<string> GetViewNames(int idONA);

        /// <summary>
        /// WebApp/GetListaValidacionEsquema: Obtiene la lista de validaciones de un esquema en un ONA.
        /// Este método permite recuperar las validaciones configuradas dentro de un esquema específico en un ONA.
        /// </summary>
        /// <param name="idONA">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <param name="idEsquemaVista">Identificador del esquema de validación dentro del ONA.</param>
        /// <returns>
        /// Devuelve una lista de objetos EsquemaVistaDto con la información de las validaciones del esquema.
        /// </returns>
        List<EsquemaVistaDto> GetListaValidacionEsquema(int idONA, int idEsquemaVista);

        /// <summary>
        /// WebApp/GetConexion: Obtiene los datos de conexión de un ONA específico.
        /// Este método permite recuperar la configuración de conexión a la base de datos de un Organismo Nacional de Acreditación.
        /// </summary>
        /// <param name="idONA">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve un objeto ONAConexion con los datos de conexión del ONA especificado.
        /// </returns>
        ONAConexion GetConexion(int idONA);

        /// <summary>
        /// WebApp/TestDatabaseConnection: Prueba la conexión a la base de datos de un ONA.
        /// Este método permite validar si una conexión a la base de datos de un Organismo Nacional de Acreditación es funcional.
        /// </summary>
        /// <param name="conexion">Objeto ONAConexion que contiene la configuración de la conexión.</param>
        /// <returns>
        /// Devuelve un valor booleano indicando si la conexión es exitosa o no.
        /// </returns>
        bool TestDatabaseConnection(ONAConexion conexion);

        /// <summary>
        /// WebApp/MigrarConexionAsync: Realiza la migración de la conexión de un ONA de forma asíncrona.
        /// Este método permite realizar el proceso de migración de conexión para un Organismo Nacional de Acreditación de manera asíncrona.
        /// </summary>
        /// <param name="idONA">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve una tarea (`Task<bool>`) indicando si la migración se completó exitosamente.
        /// </returns>
        Task<bool> MigrarConexionAsync(int idONA);


    }
}
