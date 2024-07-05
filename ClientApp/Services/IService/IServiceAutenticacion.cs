using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService
{
    public interface IServiceAutenticacion
    {
        Task<RespuestasAPI<UsuarioAutenticacionRespuestaDto>> Acceder(UsuarioAutenticacionDto usuarioAutenticacionDto);
        Task<RespuestasAPI<T>?> Recuperar<T>(UsuarioRecuperacionDto usuarioRecuperacionDto);
        Task Salir();
    }
}
