using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IBusquedaService
    {
        Task<BuscadorDto> PsBuscarPalabraAsync(string paramJSON, int PageNumber, int RowsPerPage);
        Task<List<HomologacionEsquemaDto>> FnHomologacionEsquemaTodoAsync(string idOrganizacion);
        Task<HomologacionEsquemaDto?> FnHomologacionEsquemaAsync(int idHomologacionEsquema);
        Task<List<DataHomologacionEsquema>> FnHomologacionEsquemaDatoAsync(int idHomologacionEsquema, string idOrganizacion);
        Task<List<FnPredictWordsDto>> FnPredictWords(string word);
    }
}