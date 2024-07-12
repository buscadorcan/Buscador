using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
  public interface IConexionRepository
  {

    bool Update(Conexion data);
    bool Create(Conexion data);
    Conexion? FindById(int Id);
    List<Conexion> FindAll();
  }
}