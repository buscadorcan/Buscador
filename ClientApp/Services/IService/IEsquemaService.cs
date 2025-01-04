using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService
{
    public interface IEsquemaService
    {

        Task<List<EsquemaDto>> GetListEsquemasAsync();
        Task<EsquemaDto> GetEsquemaAsync(int idEsquema);
        Task<RespuestaRegistro> RegistrarEsquemaActualizar(EsquemaDto esquemaRegistro);
        Task<bool> DeleteEsquemaAsync(int IdEsquema);
    }
}
