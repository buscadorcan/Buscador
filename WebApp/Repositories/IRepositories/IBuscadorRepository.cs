using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    /// <summary>
    /// Interfaz que define las operaciones de repositorio para realizar búsquedas y obtener esquemas de homologación en la base de datos.
    /// Esta interfaz es implementada por el repositorio de búsqueda, proporcionando acceso a funciones de búsqueda de palabras y datos asociados.
    /// </summary>
    public interface IBuscadorRepository
    {
        /// <summary>
        /// Busca una palabra en la base de datos utilizando parámetros JSON, número de página y filas por página.
        /// </summary>
        /// <param name="paramJSON">El parámetro JSON utilizado para filtrar los resultados de la búsqueda.</param>
        /// <param name="PageNumber">El número de la página para la paginación de los resultados.</param>
        /// <param name="RowsPerPage">El número de filas por página para la paginación de los resultados.</param>
        /// <returns>Un objeto <see cref="ResultPaBuscarPalabraDto"/> con los resultados de la búsqueda, incluyendo los datos y el total de registros encontrados.</returns>
        ResultPaBuscarPalabraDto BuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage);

        /// <summary>
        /// Obtiene todos los esquemas homologados disponibles para un ente específico.
        /// </summary>
        /// <param name="idEnte">El identificador del ente para el cual se deben obtener los esquemas de homologación.</param>
        /// <returns>Una lista de objetos <see cref="EsquemaDto"/> que representan los esquemas homologados disponibles.</returns>
        List<EsquemaDto> FnHomologacionEsquemaTodo(string idEnte);

        /// <summary>
        /// Obtiene los detalles de un esquema de homologación específico mediante su identificador.
        /// </summary>
        /// <param name="idHomologacionEsquema">El identificador del esquema de homologación a obtener.</param>
        /// <returns>Un objeto <see cref="FnHomologacionEsquemaDto"/> con los detalles del esquema de homologación.</returns>
        FnHomologacionEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema);

        /// <summary>
        /// Obtiene los datos asociados a un esquema de homologación específico para un ente dado.
        /// </summary>
        /// <param name="idHomologacionEsquema">El identificador del esquema de homologación.</param>
        /// <param name="idEnte">El identificador del ente para el cual se deben obtener los datos asociados al esquema.</param>
        /// <returns>Una lista de objetos <see cref="FnHomologacionEsquemaDataDto"/> con los datos de homologación asociados.</returns>
        List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idHomologacionEsquema, string idEnte);

        /// <summary>
        /// Realiza una predicción de palabras asociadas a un término de búsqueda dado.
        /// </summary>
        /// <param name="word">La palabra utilizada para predecir palabras asociadas.</param>
        /// <returns>Una lista de objetos <see cref="FnPredictWordsDto"/> que representan las palabras predichas relacionadas con la palabra de entrada.</returns>
        List<FnPredictWordsDto> FnPredictWords(string word);
    }
}
