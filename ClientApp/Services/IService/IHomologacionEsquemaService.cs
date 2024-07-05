using ClientApp.Models;

namespace ClientApp.Services.IService {
    public interface IHomologacionEsquemaService
    {
        Task<List<HomologacionEsquema>> GetHomologacionEsquemasAsync();
        Task<HomologacionEsquema> GetHomologacionEsquemaAsync(int idHomologacionEsquema);
        public Task<RespuestaRegistro> RegistrarOActualizar(HomologacionEsquema registro);
        Task<RespuestaRegistro> EliminarHomologacionEsquema(int idHomologacionEsquema);
    }
}