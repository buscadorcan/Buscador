using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IThesaurusService
    {
        /// <summary>
        /// Obtiene el thesaurus con sus términos y expansiones asociadas.
        /// </summary>
        /// <returns>Devuelve un objeto <see cref="Thesaurus"/> con la información completa del thesaurus.</returns>
        Thesaurus ObtenerThesaurus();

        /// <summary>
        /// Agrega una nueva expansión de sinónimos al thesaurus.
        /// </summary>
        /// <param name="sinonimos">Lista de sinónimos que se desean agregar como una nueva expansión.</param>
        /// <returns>Devuelve un mensaje indicando el resultado de la operación.</returns>
        string AgregarExpansion(List<string> sinonimos);

        /// <summary>
        /// Agrega un nuevo sub-elemento a una expansión existente en el thesaurus.
        /// </summary>
        /// <param name="expansionExistente">El término de expansión al que se desea agregar un sub-elemento.</param>
        /// <param name="nuevoSub">El nuevo sub-elemento a agregar en la expansión.</param>
        /// <returns>Devuelve un mensaje indicando el resultado de la operación.</returns>
        string AgregarSubAExpansion(string expansionExistente, string nuevoSub);

        /// <summary>
        /// Actualiza una lista de expansiones en el thesaurus con nuevos términos.
        /// </summary>
        /// <param name="expansions">Lista de objetos <see cref="Expansion"/> que contienen las nuevas expansiones a actualizar.</param>
        /// <returns>Devuelve un mensaje indicando el resultado de la operación.</returns>
        string ActualizarExpansion(List<Expansion> expansions);

        /// <summary>
        /// Ejecuta un archivo BAT en el servidor para procesar actualizaciones o configuraciones específicas.
        /// </summary>
        /// <returns>Devuelve un mensaje con el resultado de la ejecución del archivo BAT.</returns>
        string EjecutarArchivoBat();

        /// <summary>
        /// Actualiza el servidor de SQL Server ejecutando los procesos de reinicio o actualización configurados.
        /// </summary>
        /// <returns>Devuelve un mensaje indicando el resultado de la operación.</returns>
        string ResetSQLServer();

    }
}
