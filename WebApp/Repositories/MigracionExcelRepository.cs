using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
  public class MigracionExcelRepository : BaseRepository, IMigracionExcelRepository
  {
      private readonly IJwtService _jwtService;
       public MigracionExcelRepository(
            IJwtService jwtService,
            ILogger<MigracionExcelRepository> logger,
            ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
        }
        public MigracionExcel Create(MigracionExcel data)
        {
            data.FechaCreacion = DateTime.Now;
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
        
            return ExecuteDbOperation(context =>
            {
                context.MigracionExcel.Add(data);
                context.SaveChanges();
                return data;
            });
        }
        public MigracionExcel? FindById(int id)
        {
            return ExecuteDbOperation(context => context.MigracionExcel.AsNoTracking().FirstOrDefault(u => u.IdMigracionExcel == id));
        }
        public List<MigracionExcel> FindAll()
        {
            return ExecuteDbOperation(context => context.MigracionExcel.AsNoTracking().OrderByDescending(c => c.FechaCreacion).ToList());
        }
        public bool Update(MigracionExcel newRecord)
        {
          return ExecuteDbOperation(context => {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdMigracionExcel == newRecord.IdMigracionExcel);

                context.MigracionExcel.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
    }
}