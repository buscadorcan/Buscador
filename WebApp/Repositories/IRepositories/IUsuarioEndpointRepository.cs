using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IUsuarioEndpointRepository
  {
    ICollection<UsuarioEndpoint> FindAll();
    UsuarioEndpoint? FindByEndpointId(int endpointId);
    bool Create(UsuarioEndpoint UsuarioEndpoint);
  }
}
