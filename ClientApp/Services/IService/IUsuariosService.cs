using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IUsuariosService
    {
        Task<List<UsuarioDto>> GetUsuariosAsync();
        Task<UsuarioDto> GetUsuarioAsync(int IdUsuario);
        Task<RespuestaRegistro> RegistrarOActualizar(UsuarioDto usuarioParaRegistro);
    }
}