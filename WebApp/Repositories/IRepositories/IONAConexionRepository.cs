using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IONAConexionRepository
  {
    bool Update(ONAConexion data);
    bool Create(ONAConexion data);
    ONAConexion? FindById(int Id);
    ONAConexion? FindByIdONA(int IdONA);
    List<ONAConexion> FindAll();
  }
}
