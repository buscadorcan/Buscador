using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
    public interface IUsuarioEndpointPermisoRepository
    {
        ICollection<UsuarioEndpointPermiso> FindAll();
        UsuarioEndpointPermiso? FindByEndpointId(int endpointId);
        bool Create(UsuarioEndpointPermiso usuarioEndpointPermiso);
    }
}