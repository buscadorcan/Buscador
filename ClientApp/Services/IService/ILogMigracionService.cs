using ClientApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface ILogMigracionService
    {
        Task<List<LogMigracionDto>> GetLogMigracionesAsync();
    }
}