using WebApp.Models;

namespace WebApp.Service.IService
{
  public interface IMigrador
  {
    bool Migrar(ONAConexion conexion);
  }
}
