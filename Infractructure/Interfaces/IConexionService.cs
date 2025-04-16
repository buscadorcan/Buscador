using Infractruture.Models;
using SharedApp.Dtos;

namespace Infractruture.Interfaces {
    public interface IConexionService
    {
        Task<List<ONAConexionDto>> GetConexionsAsync();
        Task<ONAConexionDto> GetConexionAsync(int idConexion);
        public Task<RespuestaRegistro> RegistrarOActualizar(ONAConexionDto registro);
        Task<RespuestaRegistro> EliminarConexion(int idConexion);
        Task<HttpResponseMessage> ImportarExcel(MultipartFormDataContent content);
        Task<ONAConexionDto> GetOnaConexionByOnaAsync(int idOna);
        Task<List<ONAConexionDto>> GetOnaConexionByOnaListAsync(int idOna);

    }
}