using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Service
{
    public class LogMigracionService : ILogMigracionService
    {
        private readonly ILogMigracionRepository _logMigracionRepository;

        public LogMigracionService(ILogMigracionRepository logMigracionRepository)
        {
            this._logMigracionRepository = logMigracionRepository;
        }

        public LogMigracion Create(LogMigracion data)
        {
           return _logMigracionRepository.Create(data);
        }

        public Task<LogMigracion> CreateAsync(LogMigracion data)
        {
           return _logMigracionRepository.CreateAsync(data);
        }

        public LogMigracionDetalle CreateDetalle(LogMigracionDetalle data)
        {
           return _logMigracionRepository.CreateDetalle(data);
        }

        public Task<LogMigracionDetalle> CreateDetalleAsync(LogMigracionDetalle data)
        {
           return _logMigracionRepository.CreateDetalleAsync(data);
        }

        public List<LogMigracion> FindAll()
        {
           return _logMigracionRepository.FindAll();
        }

        public List<LogMigracionDetalle> FindAllDetalle()
        {
           return _logMigracionRepository.FindAllDetalle();
        }

        public LogMigracion? FindById(int Id)
        {
           return _logMigracionRepository.FindById(Id);
        }

        public List<LogMigracionDetalle> FindDetalleById(int Id)
        {
           return _logMigracionRepository.FindDetalleById(Id);
        }

        public bool Update(LogMigracion data)
        {
           return _logMigracionRepository.Update(data);
        }

        public Task<bool> UpdateAsync(LogMigracion data)
        {
           return _logMigracionRepository.UpdateAsync(data);
        }

        public bool UpdateDetalle(LogMigracionDetalle data)
        {
           return _logMigracionRepository.UpdateDetalle(data);
        }
    }
}
