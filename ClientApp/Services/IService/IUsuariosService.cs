using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IUsuariosService
    {
        Task<List<UsuarioDto>> GetUsuariosAsync();
        Task<List<VwRolDto>> GetRolesAsync();
        Task<List<OnaDto>> GetOnaAsync();
        Task<UsuarioDto> GetUsuarioAsync(int IdUsuario);
        Task<RespuestaRegistro> RegistrarOActualizar(UsuarioDto usuarioParaRegistro);
        Task<bool> DeleteUsuarioAsync(int IdUsuario);
    }
}