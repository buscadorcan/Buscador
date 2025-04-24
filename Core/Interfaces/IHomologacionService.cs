using DataAccess.Models;

namespace Core.Interfaces
{
    public interface IHomologacionService
    {
        /// <summary>
        /// Actualiza un registro de homologación en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="Homologacion"/> con los datos actualizados.</param>
        /// <returns>Devuelve un valor booleano indicando si la actualización fue exitosa.</returns>
        bool Update(Homologacion data);

        /// <summary>
        /// Crea un nuevo registro de homologación en la base de datos.
        /// </summary>
        /// <param name="data">Objeto <see cref="Homologacion"/> con los datos a insertar.</param>
        /// <returns>Devuelve un valor booleano indicando si la creación fue exitosa.</returns>
        bool Create(Homologacion data);

        /// <summary>
        /// Busca un registro de homologación en la base de datos por su identificador único.
        /// </summary>
        /// <param name="id">Identificador único de la homologación.</param>
        /// <returns>Devuelve un objeto <see cref="Homologacion"/> si se encuentra el registro; de lo contrario, devuelve null.</returns>
        Homologacion? FindById(int id);

        /// <summary>
        /// Obtiene una colección de registros de homologación asociados a un elemento superior.
        /// </summary>
        /// <returns>Devuelve una colección de objetos <see cref="Homologacion"/> relacionados con un elemento padre.</returns>
        ICollection<Homologacion> FindByParent();

        /// <summary>
        /// Busca una lista de registros de homologación en la base de datos utilizando múltiples identificadores.
        /// </summary>
        /// <param name="ids">Array de identificadores únicos de homologaciones.</param>
        /// <returns>Devuelve una lista de objetos <see cref="Homologacion"/> encontrados en la base de datos.</returns>
        List<Homologacion> FindByIds(int[] ids);

        /// <summary>
        /// Obtiene la homologación en base a un código de homologación específico.
        /// </summary>
        /// <param name="codigoHomologacion">Código único de homologación.</param>
        /// <returns>Devuelve una lista de objetos <see cref="VwHomologacion"/> con la información correspondiente al código ingresado.</returns>
        List<VwHomologacion> ObtenerVwHomologacionPorCodigo(string codigoHomologacion);

        /// <summary>
        /// Obtiene la lista completa de registros de homologación almacenados en la base de datos.
        /// </summary>
        /// <returns>Devuelve una lista de todos los registros de <see cref="Homologacion"/> en la base de datos.</returns>
        List<Homologacion> FindByAll();
    }
}
