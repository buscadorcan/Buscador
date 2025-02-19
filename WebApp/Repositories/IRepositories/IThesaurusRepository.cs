using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IThesaurusRepository
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerThesaurus: Obtiene la información completa del thesaurus almacenado en la base de datos.
         */
        Thesaurus ObtenerThesaurus();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GuardarThesaurus: Guarda o actualiza el thesaurus en la base de datos.
         */
        void GuardarThesaurus(Thesaurus thesaurus);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/EjecutarArchivoBat: Ejecuta un archivo .bat en el servidor para automatizar procesos relacionados con el thesaurus.
         */
        void EjecutarArchivoBat();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ResetSQLServer: actualiza el servidor de sqlserver*/
        void ResetSQLServer();

    }
}
