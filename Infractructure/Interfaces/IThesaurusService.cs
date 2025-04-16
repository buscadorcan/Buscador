using Infractruture.Models;
using SharedApp.Dtos;

namespace Infractruture.Interfaces
{
    public interface IThesaurusService
    {
        Task<ThesaurusDto> GetThesaurusAsync(string endpoint);
        Task<RespuestaRegistro> UpdateExpansionAsync(string endpoint, List<ExpansionDto> expansions);
        Task<string> EjecutarBatAsync(string endpoint);
        Task<string> ResetSqlServerAsync(string endpoint);
    }
}
