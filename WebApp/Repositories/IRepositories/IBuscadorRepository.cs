using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    public interface IBuscadorRepository
    {
        BuscadorDto PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage);
        List<EsquemaDto> FnHomologacionEsquemaTodo(string VistaFK, int idOna);
        FnEsquemaDto? FnHomologacionEsquema(int idHomologacionEsquema);
        List<FnHomologacionEsquemaDataDto> FnHomologacionEsquemaDato(int idEsquema, string VistaFK, int idOna);
        List<FnEsquemaDataBuscadoDto> FnEsquemaDatoBuscar(int idOna, int idEsquema, string VistaPK, string TextoBuscar);
        List<FnPredictWordsDto> FnPredictWords(string word);
    }
}
