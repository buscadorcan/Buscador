using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IMigracionExcelService
    {
        Task<List<MigracionExcelDto>> GetMigracionExcelsAsync();
        Task<HttpResponseMessage> ImportarExcel(MultipartFormDataContent content, int idOna);
    }
}