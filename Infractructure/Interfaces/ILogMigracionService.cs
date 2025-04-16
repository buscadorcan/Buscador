using Infractruture.Models;
using SharedApp.Dtos;

namespace Infractruture.Interfaces {
    public interface ILogMigracionService
    {
        Task<List<LogMigracionDto>> GetLogMigracionesAsync();
    }
}