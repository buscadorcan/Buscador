using DataAccess.Models;

namespace DataAccess.Interfaces
{
    public interface IThesaurusRepository
    {
        /// <summary>
        /// Obtiene la información completa del thesaurus almacenado en la base de datos.
        /// </summary>
        /// <returns>Devuelve un objeto de tipo <see cref="Thesaurus"/> que contiene la información del thesaurus.</returns>
        Thesaurus ObtenerThesaurus();

        /// <summary>
        /// Guarda o actualiza el thesaurus en la base de datos.
        /// </summary>
        /// <param name="thesaurus">El objeto <see cref="Thesaurus"/> que se almacenará o actualizará en la base de datos.</param>
        void GuardarThesaurus(Thesaurus thesaurus);

        /// <summary>
        /// Ejecuta un archivo .bat en el servidor para automatizar procesos relacionados con el thesaurus.
        /// </summary>
        void EjecutarArchivoBat();

        /// <summary>
        /// Reinicia el servidor de SQL Server y actualiza su estado.
        /// </summary>
        string ResetSQLServer();


    }
}
