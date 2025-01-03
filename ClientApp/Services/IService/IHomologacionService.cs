using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IHomologacionService
    {
        Task<List<HomologacionDto>> GetHomologacionsAsync(int valor);
        Task<HomologacionDto> GetHomologacionAsync(int idHomologacion);
        Task<RespuestaRegistro> RegistrarOActualizar(HomologacionDto registro);
        Task<RespuestaRegistro> EliminarHomologacion(int idHomologacion);
    }
}