using DataAccess.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedApp.Dtos;


namespace DataAccess.Repositories
{
    public class OnaMigrateRepository : BaseRepository, IOnaMigrateRepository
    {

        public OnaMigrateRepository(
          ILogger<EventTrackingRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idOna"></param>
        /// <param name="idEsquema"></param>
        /// <param name="jsonParameter"></param>
        /// <returns></returns>
        public List<OnaMigrateDto> postOnaMigrate(int idOna, int idEsquema, string jsonParameter)
        {
            return ExecuteDbOperation(context =>
            {
                try
                {
                    string sql = $"EXEC GenerarEsquemaData @idOna, @idEsquema, @jsonParameter";
                    return context.onaMigrate
                    .FromSqlRaw(sql,
                           new SqlParameter("@idOna", idOna),
                           new SqlParameter("@idEsquema", idEsquema),
                           new SqlParameter("@jsonParameter", jsonParameter))
                       .ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing query postOnaMigrate");
                    return [];
                }
            });
        }
    }
}
