using Infractruture.Models;
using SharedApp.Dtos;

namespace Infractruture.Interfaces {
    public interface IHomologacionEsquemaService
    {
        Task<List<HomologacionEsquemaDto>> GetHomologacionEsquemasAsync();
        Task<HomologacionEsquemaDto> GetHomologacionEsquemaAsync(int idHomologacionEsquema);
        public Task<RespuestaRegistro> RegistrarOActualizar(HomologacionEsquemaDto registro);
        Task<RespuestaRegistro> EliminarHomologacionEsquema(int idHomologacionEsquema);
    }
}