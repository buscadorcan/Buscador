using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SharedApp.Models.Dtos;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class EventTrackingRepository : BaseRepository, IEventTrackingRepository
    {
        public EventTrackingRepository(
          ILogger<EventTrackingRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
            
        }
        /// <summary>
        /// Executes a stored procedure to add event tracking data to the database.
        /// </summary>
        /// <param name="data">The data transfer object containing event tracking information.</param>
        /// <returns>
        /// <c>true</c> if the operation was successful; otherwise, <c>false</c>.
        /// </returns>
        public bool Create(paAddEventTrackingDto data)
        {
            return ExecuteDbOperation(context =>
            {
                try {
                    return context.Database.SqlQueryRaw<bool>(
                        "exec paAddEventTracking @TipoUsuario, @NombreUsuario, @NombrePagina, @NombreControl, @NombreAccion, @UbicacionJson, @ParametroJson",
                        new SqlParameter("@TipoUsuario", data.TipoUsuario),
                        new SqlParameter("@NombreUsuario", data.NombreUsuario),
                        new SqlParameter("@NombrePagina", data.NombrePagina),
                        new SqlParameter("@NombreControl", data.NombreControl),
                        new SqlParameter("@NombreAccion", data.NombreAccion),
                        new SqlParameter("@UbicacionJson", data.UbicacionJson),
                        new SqlParameter("@ParametroJson", data.ParametroJson)
                    ).AsEnumerable().FirstOrDefault();
                } catch(Exception ex) {
                    _logger.LogError(ex, "Error executing stored procedure paAddEventTracking");
                    return false;
                }
            });
        }
    }
}