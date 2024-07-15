using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IBusquedaService
    {
        Task<ResultDataHomologacionEsquema> PsBuscarPalabraAsync(string paramJSON, int PageNumber, int RowsPerPage);
        Task<List<HomologacionEsquemaDto>> FnHomologacionEsquemaTodoAsync();
        Task<HomologacionEsquemaDto?> FnHomologacionEsquemaAsync(int idHomologacionEsquema);
        Task<List<DataHomologacionEsquema>> FnHomologacionEsquemaDatoAsync(int idHomologacionEsquema, int idDataLakeOrganizacion);
        Task<List<FnPredictWordsDto>> FnPredictWords(string word);
    }
}