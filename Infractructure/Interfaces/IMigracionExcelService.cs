using Infractruture.Models;
using SharedApp.Dtos;

namespace Infractruture.Interfaces {
    public interface IMigracionExcelService
    {
        Task<List<MigracionExcelDto>> GetMigracionExcelsAsync();
        Task<HttpResponseMessage> ImportarExcel(MultipartFormDataContent content, int idOna);
    }
}