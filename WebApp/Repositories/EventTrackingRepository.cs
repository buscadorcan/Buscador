using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharedApp.Models.Dtos;
using WebApp.Models;
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
                        "exec paAddEventTracking @CodigoHomologacionRol, @NombreUsuario, @CodigoHomologacionMenu, @NombreControl, @NombreAccion, @UbicacionJson, @ParametroJson",
                        new SqlParameter("@CodigoHomologacionRol", data.CodigoHomologacionRol),
                        new SqlParameter("@NombreUsuario", data.NombreUsuario),
                        new SqlParameter("@CodigoHomologacionMenu", data.CodigoHomologacionMenu),
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

        public Menus? FindDataById(int idHRol, int idHMenu)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string GetCodeByUser(string nombreUsuario, string codigoHomologacionRol, string codigoHomologacionMenu)
        {
            return ExecuteDbOperation(context => {
                var result = context.EventTracking.AsNoTracking()
                    .Where(u => u.NombreUsuario == nombreUsuario && u.CodigoHomologacionRol == codigoHomologacionRol && u.CodigoHomologacionMenu == codigoHomologacionMenu)
                    .OrderByDescending(o => o.FechaCreacion)
                    .FirstOrDefault();

                return result != null ? JsonConvert.DeserializeObject<JObject>(result.ParametroJson)?["Code"]?.ToString() : null;
            });
        }
    }
}