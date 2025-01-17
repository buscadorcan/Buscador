using ClientApp.Models;
using SharedApp.Models.Dtos;
using System.Collections.Generic;

namespace ClientApp.Services.IService
{
    public interface IEsquemaService
    {

        Task<List<EsquemaDto>> GetListEsquemasAsync();
        Task<EsquemaDto> GetEsquemaAsync(int idEsquema);
        Task<RespuestaRegistro> RegistrarEsquemaActualizar(EsquemaDto esquemaRegistro);
        Task<bool> DeleteEsquemaAsync(int IdEsquema);
        Task<List<EsquemaVistaOnaDto>> GetEsquemaByOnaAsync(int idOna);
        Task<RespuestaRegistro> GuardarEsquemaVistaValidacionAsync(EsquemaVistaValidacionDto esquemaRegistro);
    }
}
