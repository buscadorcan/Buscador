using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    public interface IBuscadorRepository
    {
         /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/PsBuscarPalabra: Realiza una búsqueda de palabras clave en la base de datos y devuelve los resultados paginados.
         */
        BuscadorDto PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnHomologacionEsquemaTodo: Obtiene el esquema completo de homologación basado en la vista y el ONA.
         */
        List<EsquemaDto> FnHomologacionEsquemaTodo(string VistaFK, int idOna);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnHomologacionEsquema: Obtiene los detalles de homologación de un esquema específico.
         */
        FnEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnHomologacionEsquemaDato: Recupera los datos de homologación de un esquema en base a la vista y el ONA.
         */
        List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idEsquema, string VistaFK, int idOna);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnEsquemaDatoBuscar: Realiza una búsqueda de datos dentro de un esquema específico.
         */
        List<FnEsquemaDataBuscadoDto> FnEsquemaDatoBuscar(int IdEsquemaData, string TextoBuscar);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnPredictWords: Genera predicciones de palabras relacionadas con la consulta ingresada.
         */
        List<FnPredictWordsDto> FnPredictWords(string word);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ValidateWords: Valida una lista de palabras en base a las reglas establecidas en el sistema.
         */
        bool ValidateWords(List<string> words);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnEsquemaCabecera: Obtiene la información de la cabecera de un esquema específico.
         */
        fnEsquemaCabeceraDto? FnEsquemaCabecera(int IdEsquemadata);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/AddEventTracking: Registra un evento de seguimiento en el sistema.
         */
        void AddEventTracking(EventTrackingDto eventTracking);

    }
}
