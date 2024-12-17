using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IONAConexionRepository
  {
    bool Update(ONAConexion data);
    bool Create(ONAConexion data);
    ONAConexion? FindById(int Id);
    ONAConexion? FindBySiglas(string siglas);
    List<ONAConexion> FindAll();
  }
}
