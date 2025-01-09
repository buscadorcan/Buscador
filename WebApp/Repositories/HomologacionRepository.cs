using System.Data;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class HomologacionRepository : BaseRepository, IHomologacionRepository
    {
        private readonly IJwtService _jwtService;
        public HomologacionRepository(
          IJwtService jwtService,
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
        }
        public bool Create(Homologacion data)
        {
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            data.IdUserModifica = data.IdUserCreacion;
            data.Estado = "A";
            data.Mostrar = "S";
            return ExecuteDbOperation(context =>
            {
                context.Homologacion.Add(data);
                return context.SaveChanges() >= 0;
            });
        }
        public Homologacion? FindById(int id)
        {
            return ExecuteDbOperation(context => context.Homologacion.AsNoTracking().FirstOrDefault(u => u.IdHomologacion == id));
        }
        public ICollection<Homologacion> FindByParent()
        {
            return ExecuteDbOperation(context =>
            {
                // Encuentra el IdHomologacion basado en el código en este caso KEY_DIM_ESQUEMA
                var parentId = context.Homologacion
                    .Where(h => h.CodigoHomologacion == "KEY_DIM_ESQUEMA")
                    .Select(h => h.IdHomologacion)
                    .FirstOrDefault();

                // Filtra por IdHomologacionGrupo y Estado
                return context.Homologacion
                    .Where(h => h.IdHomologacionGrupo == parentId && h.Estado == "A")
                    .OrderBy(h => h.MostrarWebOrden)
                    .AsNoTracking() // Similar a NOLOCK
                    .ToList();
            });
        }
        public List<Homologacion> FindByIds(int[] ids)
        {
            return ExecuteDbOperation(context => context.Homologacion.Where(c => ids.Contains(c.IdHomologacion)).OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public bool Update(Homologacion newRecord)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdHomologacion == newRecord.IdHomologacion);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

                context.Homologacion.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
        /// <inheritdoc />
        public List<VwHomologacion> ObtenerVwHomologacionPorCodigo(string codigoHomologacion)
        {
            return ExecuteDbOperation(context =>
              context.VwHomologacion
                .AsNoTracking()
                .Where(c => c.CodigoHomologacionKEY == codigoHomologacion)
                .OrderBy(c => c.MostrarWeb)
                .ToList());
        }
    }
}
