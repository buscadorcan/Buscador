using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService
{
    public interface IONAService
    {
        Task<List<ONADto>> GetONAsAsync();
        Task<ONADto> GetONAsAsync(int IdONA);
        Task<RespuestaRegistro> RegistrarONAsActualizar(ONADto ONAParaRegistro);
    }
}
