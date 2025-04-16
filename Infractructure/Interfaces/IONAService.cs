using Infractruture.Models;
using SharedApp.Dtos;

namespace Infractruture.Interfaces
{
    public interface IONAService
    {
        Task<List<OnaDto>> GetONAsAsync();

        Task<List<VwPaisDto>> GetPaisesAsync();

        Task<OnaDto> GetONAsAsync(int IdONA);

        Task<RespuestaRegistro> RegistrarONAsActualizar(OnaDto ONAParaRegistro);

        Task<bool> DeleteONAAsync(int IdONA);

        Task<List<OnaDto>> GetListByONAsAsync(int idOna);
    }
}