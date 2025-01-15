using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IMigrador
    {
        Task<Boolean> MigrarAsync(ONAConexion conexion);
    }
}
