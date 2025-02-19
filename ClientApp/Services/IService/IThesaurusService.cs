using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService
{
    public interface IThesaurusService
    {
        Task<ThesaurusDto> GetThesaurusAsync(string endpoint);
        Task<RespuestaRegistro> UpdateExpansionAsync(string endpoint, List<ExpansionDto> expansions);
        Task<string> EjecutarBatAsync(string endpoint);
        Task<string> ResetSqlServerAsync(string endpoint);
    }
}
