using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IBusquedaService
    {
        Task<BuscadorDto> PsBuscarPalabraAsync(string paramJSON, int PageNumber, int RowsPerPage);
        Task<List<HomologacionEsquemaDto>> FnHomologacionEsquemaTodoAsync(string vistaFK, int idOna);
        Task<HomologacionEsquemaDto?> FnHomologacionEsquemaAsync(int idHomologacionEsquema);
        Task<List<DataHomologacionEsquema>> FnHomologacionEsquemaDatoAsync(int idHomologacionEsquema, string VistaFK, int idOna);
        Task<List<FnPredictWordsDto>> FnPredictWords(string word);
    }
}