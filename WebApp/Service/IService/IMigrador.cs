using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IMigrador
    {
        Boolean Migrar(Conexion conexion);
    }
}