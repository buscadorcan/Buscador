
using WebApp.Models;

namespace WebApp.Service.IService
{
  public interface IConectionStringBuilderService
  {
    string BuildConnectionString(ONAConexion conexion);
  }
}
