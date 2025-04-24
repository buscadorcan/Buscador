using DataAccess.Models;

namespace Core.Interfaces
{
    public interface ILogMigracionService
    {
        /// <summary>
        /// Actualiza un registro de LogMigración en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracion"/> con los datos actualizados.</param>
        /// <returns>Devuelve un valor booleano indicando si la actualización fue exitosa.</returns>
        bool Update(LogMigracion data);

        /// <summary>
        /// Actualiza un registro de LogMigración de forma asíncrona en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracion"/> con los datos actualizados.</param>
        /// <returns>Devuelve un <see cref="Task{bool}"/> con el resultado de la operación.</returns>
        Task<bool> UpdateAsync(LogMigracion data);

        /// <summary>
        /// Crea un nuevo registro de LogMigración en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracion"/> con los datos a insertar.</param>
        /// <returns>Devuelve el objeto <see cref="LogMigracion"/> creado.</returns>
        LogMigracion Create(LogMigracion data);

        /// <summary>
        /// Crea un nuevo registro de LogMigración de forma asíncrona en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracion"/> con los datos a insertar.</param>
        /// <returns>Devuelve un <see cref="Task{LogMigracion}"/> con el objeto creado.</returns>
        Task<LogMigracion> CreateAsync(LogMigracion data);

        /// <summary>
        /// Busca un registro de LogMigración en la base de datos por su identificador único.
        /// </summary>
        /// <param name="Id">Identificador único del registro de LogMigración.</param>
        /// <returns>Devuelve un objeto <see cref="LogMigracion"/> si se encuentra el registro; de lo contrario, devuelve null.</returns>
        LogMigracion? FindById(int Id);

        /// <summary>
        /// Obtiene la lista completa de registros de LogMigración almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve una lista de objetos <see cref="LogMigracion"/>.</returns>
        List<LogMigracion> FindAll();

        /// <summary>
        /// Actualiza un registro de detalle de LogMigración en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracionDetalle"/> con los datos actualizados.</param>
        /// <returns>Devuelve un valor booleano indicando si la actualización fue exitosa.</returns>
        bool UpdateDetalle(LogMigracionDetalle data);


        /// <summary>
        /// Crea un nuevo registro de detalle de LogMigración en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracionDetalle"/> con los datos a insertar.</param>
        /// <returns>Devuelve el objeto <see cref="LogMigracionDetalle"/> creado.</returns>
        LogMigracionDetalle CreateDetalle(LogMigracionDetalle data);

        /// <summary>
        /// Crea un nuevo registro de detalle de LogMigración de forma asíncrona en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracionDetalle"/> con los datos a insertar.</param>
        /// <returns>Devuelve un <see cref="Task{LogMigracionDetalle}"/> con el objeto creado.</returns>
        Task<LogMigracionDetalle> CreateDetalleAsync(LogMigracionDetalle data);

        /// <summary>
        /// Obtiene la lista completa de registros de detalles de LogMigración almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve una lista de objetos <see cref="LogMigracionDetalle"/>.</returns>
        List<LogMigracionDetalle> FindAllDetalle();

        /// <summary>
        /// Busca los detalles de un registro de LogMigración en la base de datos por su identificador.
        /// </summary>
        /// <param name="Id">Identificador único del registro de LogMigración.</param>
        /// <returns>Devuelve una lista de objetos <see cref="LogMigracionDetalle"/> correspondientes al registro especificado.</returns>
        List<LogMigracionDetalle> FindDetalleById(int Id);

    }
}
