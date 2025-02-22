using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService
{
    public interface IMenuService
    {
        Task<List<MenuRolDto>> GetMenusAsync();
        Task<MenuRolDto> GetMenuAsync(int idHRol, int idHMenu);
        Task<RespuestaRegistro> RegistrarMenuActualizar(MenuRolDto menuParaRegistro);
        Task<bool> DeleteMenuAsync(int? idHRol, int? idHMenu);
        Task<List<MenuPaginaDto>> GetMenusPendingConfigAsync(int? idHomologacionRol);

    }
}