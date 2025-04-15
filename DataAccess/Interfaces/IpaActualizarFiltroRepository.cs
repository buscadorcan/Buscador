namespace DataAccess.Interfaces
{
    public interface IpaActualizarFiltroRepository
    {
        /// <summary>
        /// Actualiza de forma asíncrona los filtros almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve un <see cref="Task{Boolean}"/> que indica si la operación fue exitosa.</returns>
        Task<bool> ActualizarFiltroAsync();

    }
}
