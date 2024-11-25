using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IConexionService
    {
        Task<List<ConexionDto>> GetConexionsAsync();
        Task<ConexionDto> GetConexionAsync(int idConexion);
        public Task<RespuestaRegistro> RegistrarOActualizar(ConexionDto registro);
        Task<RespuestaRegistro> EliminarConexion(int idConexion);
        Task<HttpResponseMessage> ImportarExcel(MultipartFormDataContent content);
    }
}