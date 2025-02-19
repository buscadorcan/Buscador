using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IThesaurusService
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerThesaurus: Obtiene el thesaurus con sus términos y expansiones asociadas.
         */
        Thesaurus ObtenerThesaurus();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/AgregarExpansion: Agrega una nueva expansión de sinónimos al thesaurus.
         */
        string AgregarExpansion(List<string> sinonimos);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/AgregarSubAExpansion: Agrega un nuevo sub-elemento a una expansión existente en el thesaurus.
         */
        string AgregarSubAExpansion(string expansionExistente, string nuevoSub);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ActualizarExpansion: Actualiza una lista de expansiones en el thesaurus con nuevos términos.
         */
        string ActualizarExpansion(List<Expansion> expansions);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/EjecutarArchivoBat: Ejecuta un archivo BAT en el servidor para procesar actualizaciones o configuraciones específicas.
         */
        string EjecutarArchivoBat();

    }
}
