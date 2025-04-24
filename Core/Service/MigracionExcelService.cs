using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Service
{
    public class MigracionExcelService: IMigracionExcelService
    {
        private readonly IMigracionExcelRepository _migracionExcelRepository;

        public MigracionExcelService(IMigracionExcelRepository migracionExcelRepository)
        {
            this._migracionExcelRepository = migracionExcelRepository;
        }

        public LogMigracion Create(LogMigracion data)
        {
            return _migracionExcelRepository.Create(data);
        }

        public Task<LogMigracion> CreateAsync(LogMigracion data)
        {
            return _migracionExcelRepository.CreateAsync(data);
        }

        public List<MigracionExcel> FindAll()
        {
            return _migracionExcelRepository.FindAll();
        }

        public MigracionExcel? FindById(int Id)
        {
            return _migracionExcelRepository.FindById(Id);
        }

        public bool Update(LogMigracion data)
        {
            return _migracionExcelRepository.Update(data);
        }

        public Task<bool> UpdateAsync(LogMigracion data)
        {
            return _migracionExcelRepository.UpdateAsync(data);
        }
    }
}
