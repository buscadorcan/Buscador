using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IConexionService
    {
        Task<List<ONAConexionDto>> GetConexionsAsync();
        Task<ONAConexionDto> GetConexionAsync(int idConexion);
        public Task<RespuestaRegistro> RegistrarOActualizar(ONAConexionDto registro);
        Task<RespuestaRegistro> EliminarConexion(int idConexion);
        Task<HttpResponseMessage> ImportarExcel(MultipartFormDataContent content);
        Task<ONAConexionDto> GetOnaConexionByOnaAsync(int idOna);

    }
}