using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    public interface IBuscadorRepository
    {
        BuscadorDto PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage);
        List<EsquemaDto> FnHomologacionEsquemaTodo(string VistaFK, int idOna);
        FnEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema);
        List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idEsquema, string VistaFK, int idOna);
        List<FnPredictWordsDto> FnPredictWords(string word);
    }
}
