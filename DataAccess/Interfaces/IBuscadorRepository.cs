using SharedApp.Dtos;

namespace DataAccess.Interfaces
{
    public interface IBuscadorRepository
    {
        /// <summary>
        /// WebApp/PsBuscarPalabra: Realiza una búsqueda de palabras clave en la base de datos y devuelve los resultados paginados.
        /// Este método permite buscar términos específicos dentro del sistema y obtener una lista de resultados con paginación.
        /// </summary>
        /// <param name="paramJSON">Parámetros en formato JSON para personalizar la búsqueda.</param>
        /// <param name="PageNumber">Número de página para la paginación de resultados.</param>
        /// <param name="RowsPerPage">Cantidad de resultados por página.</param>
        /// <returns>
        /// Devuelve un objeto de tipo BuscadorDto con los resultados paginados de la búsqueda.
        /// </returns>
        BuscadorDto PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage);

        /// <summary>
        /// WebApp/FnHomologacionEsquemaTodo: Obtiene el esquema completo de homologación basado en la vista y el ONA.
        /// Este método permite recuperar la estructura completa de homologación asociada a una vista específica y un Organismo Nacional de Acreditación (ONA).
        /// </summary>
        /// <param name="VistaFK">Nombre de la vista de homologación a consultar.</param>
        /// <param name="idOna">Identificador del ONA asociado a la vista de homologación.</param>
        /// <returns>
        /// Devuelve una lista de objetos EsquemaDto con la información del esquema de homologación.
        /// </returns>
        List<EsquemaDto> FnHomologacionEsquemaTodo(string VistaFK, int idOna);

        /// <summary>
        /// WebApp/FnHomologacionEsquema: Obtiene los detalles de homologación de un esquema específico.
        /// Este método permite consultar información detallada sobre un esquema de homologación a partir de su identificador único.
        /// </summary>
        /// <param name="idHomologacionEsquema">ID único del esquema de homologación.</param>
        /// <returns>
        /// Devuelve un objeto de tipo FnEsquemaDto con la información detallada del esquema de homologación.
        /// </returns>
        FnEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema);

        /// <summary>
        /// WebApp/FnHomologacionEsquemaDato: Recupera los datos de homologación de un esquema en base a la vista y el ONA.
        /// Este método permite obtener los datos asociados a un esquema de homologación en función de su vista de referencia y ONA.
        /// </summary>
        /// <param name="idEsquema">Identificador del esquema a consultar.</param>
        /// <param name="VistaFK">Nombre de la vista asociada al esquema de homologación.</param>
        /// <param name="idOna">Identificador del ONA correspondiente.</param>
        /// <returns>
        /// Devuelve una lista de objetos FnHomologacionEsquemaDataDto con los datos de homologación recuperados.
        /// </returns>
        List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idEsquema, string VistaFK, int idOna);

        /// <summary>
        /// WebApp/FnEsquemaDatoBuscar: Realiza una búsqueda de datos dentro de un esquema específico.
        /// Este método permite buscar información dentro de un esquema determinado utilizando un término de búsqueda.
        /// </summary>
        /// <param name="IdEsquemaData">Identificador del esquema en el que se realizará la búsqueda.</param>
        /// <param name="TextoBuscar">Término de búsqueda para filtrar los resultados.</param>
        /// <returns>
        /// Devuelve una lista de objetos FnEsquemaDataBuscadoDto con los datos que coinciden con la búsqueda realizada.
        /// </returns>
        List<FnEsquemaDataBuscadoDto> FnEsquemaDatoBuscar(int IdEsquemaData, string TextoBuscar);

        /// <summary>
        /// WebApp/FnPredictWords: Genera predicciones de palabras relacionadas con la consulta ingresada.
        /// Este método permite obtener sugerencias de palabras a partir de un prefijo o término parcial ingresado.
        /// </summary>
        /// <param name="word">Palabra o término parcial para generar las predicciones.</param>
        /// <returns>
        /// Devuelve una lista de objetos FnPredictWordsDto con las palabras sugeridas basadas en la entrada proporcionada.
        /// </returns>
        List<FnPredictWordsDto> FnPredictWords(string word);

        /// <summary>
        /// WebApp/ValidateWords: Valida una lista de palabras en base a las reglas establecidas en el sistema.
        /// Este método permite verificar si una serie de palabras cumplen con las normas y restricciones del sistema.
        /// </summary>
        /// <param name="words">Lista de palabras a validar.</param>
        /// <returns>
        /// Devuelve un valor booleano indicando si todas las palabras cumplen con las reglas establecidas.
        /// </returns>
        bool ValidateWords(List<string> words);

        /// <summary>
        /// WebApp/FnEsquemaCabecera: Obtiene la información de la cabecera de un esquema específico.
        /// Este método permite recuperar datos generales sobre un esquema a partir de su identificador único.
        /// </summary>
        /// <param name="IdEsquemadata">ID único del esquema cuya cabecera se desea consultar.</param>
        /// <returns>
        /// Devuelve un objeto fnEsquemaCabeceraDto con la información de la cabecera del esquema solicitado.
        /// Si el esquema no existe, devuelve un valor nulo.
        /// </returns>
        fnEsquemaCabeceraDto? FnEsquemaCabecera(int IdEsquemadata);

        /// <summary>
        /// WebApp/AddEventTracking: Registra un evento de seguimiento en el sistema.
        /// Este método permite almacenar un evento en la base de datos para su posterior análisis.
        /// </summary>
        /// <param name="eventTracking">Objeto que contiene la información del evento de seguimiento a registrar.</param>
        int AddEventTracking(EventTrackingDto eventTracking);


    }
}
