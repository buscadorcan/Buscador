using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService
{
    public interface IONAService
    {
        Task<List<ONADto>> GetONAsAsync();
        Task<List<VwPaisDto>> GetPaisesAsync();
        Task<ONADto> GetONAsAsync(int IdONA);
        Task<RespuestaRegistro> RegistrarONAsActualizar(ONADto ONAParaRegistro);
    }
}
