using Infractruture.Models;
using SharedApp.Dtos;

namespace Infractruture.Interfaces {
    public interface IHomologacionService
    {
        Task<List<HomologacionDto>> GetHomologacionsAsync();
        Task<HomologacionDto> GetHomologacionAsync(int idHomologacion);
        Task<RespuestaRegistro> RegistrarOActualizar(HomologacionDto registro);
        Task<RespuestaRegistro> EliminarHomologacion(int idHomologacion);
        Task<bool> DeleteHomologacionAsync(int idHomologacion);
        Task<List<HomologacionDto>> GetHomologacionsSelectAsync(string codigoHomologacion);
        Task<List<HomologacionDto>> GetFindByAllAsync();

    }
}