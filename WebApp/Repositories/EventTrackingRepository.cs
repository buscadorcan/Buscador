using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        /// <inheritdoc />
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

        /// <inheritdoc />
        public string GetCodeByUser(string nombreUsuario, string tipoUsuario, string nombrePagina)
        {
            return ExecuteDbOperation(context => {
                var result = context.EventTracking.AsNoTracking()
                    .Where(u => u.NombreUsuario == nombreUsuario && u.TipoUsuario == tipoUsuario && u.NombrePagina == nombrePagina)
                    .OrderByDescending(o => o.FechaCreacion)
                    .FirstOrDefault();

                return result != null ? JsonConvert.DeserializeObject<JObject>(result.ParametroJson)?["Code"]?.ToString() : null;
            });
        }
    }
}