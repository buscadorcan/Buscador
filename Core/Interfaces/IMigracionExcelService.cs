using DataAccess.Models;

namespace Core.Interfaces
{
    public interface IMigracionExcelService
    {
        /// <summary>
        /// Actualiza un registro de <see cref="LogMigracion"/> en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracion"/> con los datos actualizados.</param>
        /// <returns>Devuelve <c>true</c> si la actualización fue exitosa, de lo contrario <c>false</c>.</returns>
        bool Update(LogMigracion data);

        /// <summary>
        /// Actualiza un registro de <see cref="LogMigracion"/> de forma asíncrona en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracion"/> con los datos actualizados.</param>
        /// <returns>Devuelve un <see cref="Task{bool}"/> indicando el éxito de la operación.</returns>
        Task<bool> UpdateAsync(LogMigracion data);

        /// <summary>
        /// Crea un nuevo registro de <see cref="LogMigracion"/> en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracion"/> con los datos a insertar.</param>
        /// <returns>Devuelve el objeto <see cref="LogMigracion"/> creado.</returns>
        LogMigracion Create(LogMigracion data);

        /// <summary>
        /// Crea un nuevo registro de <see cref="LogMigracion"/> de forma asíncrona en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="LogMigracion"/> con los datos a insertar.</param>
        /// <returns>Devuelve un <see cref="Task{LogMigracion}"/> con el objeto creado.</returns>
        Task<LogMigracion> CreateAsync(LogMigracion data);

        /// <summary>
        /// Busca un registro de <see cref="MigracionExcel"/> en la base de datos por su identificador único.
        /// </summary>
        /// <param name="Id">Identificador único del registro.</param>
        /// <returns>Devuelve un objeto <see cref="MigracionExcel"/> si se encuentra, de lo contrario <c>null</c>.</returns>
        MigracionExcel? FindById(int Id);

        /// <summary>
        /// Obtiene la lista completa de registros de <see cref="MigracionExcel"/> almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve una lista de objetos <see cref="MigracionExcel"/>.</returns>
        List<MigracionExcel> FindAll();
    }
}
