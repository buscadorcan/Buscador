using System.Data;
using Microsoft.EntityFrameworkCore;
using SharedApp.Models.Dtos;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class EsquemaRepository : BaseRepository, IEsquemaRepository
    {
        private readonly IJwtService _jwtService;
        public EsquemaRepository(
          IJwtService jwtService,
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
        }
        public bool Create(Esquema data)
        {
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            data.IdUserModifica = data.IdUserCreacion;
            data.FechaCreacion = DateTime.Now;
            data.FechaModifica = DateTime.Now;
            data.Estado = "A";

            return ExecuteDbOperation(context =>
            {
                context.Esquema.Add(data);
                return context.SaveChanges() >= 0;
            });
        }
        public bool CreateEsquemaValidacion(EsquemaVista data)
        {
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            data.IdUserModifica = data.IdUserCreacion;
            data.FechaModifica = DateTime.Now;
            data.Estado = "A";

            return ExecuteDbOperation(context =>
            {
                context.EsquemaVista.Add(data);
                return context.SaveChanges() >= 0;
            });
        }
        public Esquema? FindById(int id)
        {
            return ExecuteDbOperation(context => context.Esquema.AsNoTracking().FirstOrDefault(u => u.IdEsquema == id));
        }
        public EsquemaVistaColumna? GetEsquemaVistaColumnaByIdEquemaVistaAsync(int id)
        {
            return ExecuteDbOperation(context => context.EsquemaVistaColumna.AsNoTracking().FirstOrDefault(u => u.IdEsquemaVista == id));
        }
        public Esquema? FindByViewName(string esquemaVista)
        {
            return ExecuteDbOperation(context => context.Esquema.AsNoTracking().FirstOrDefault(u => u.EsquemaVista == esquemaVista));
        }
        public List<Esquema> FindAll()
        {
            return ExecuteDbOperation(context => context.Esquema.AsNoTracking().Where(c => c.Estado.Equals("A")).OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public List<Esquema> FindAllWithViews()
        {
            return ExecuteDbOperation(context => context.Esquema.AsNoTracking().Where(c => c.Estado == "A" && c.EsquemaVista != null && !string.IsNullOrEmpty(c.EsquemaVista.Trim())).OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public bool Update(Esquema newRecord)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdEsquema == newRecord.IdEsquema);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

                context.Esquema.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
        public bool EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(int IdEsquemaVista)
        {
            return ExecuteDbOperation(context =>
            {
                // Obtener todos los registros que coincidan con el IdEsquemaVista
                var registros = context.EsquemaVistaColumna
                    .Where(e => e.IdEsquemaVista == IdEsquemaVista)
                    .ToList();

                if (registros.Any())
                {
                    foreach (var registro in registros)
                    {
                        registro.Estado = "X"; // Actualizar el estado
                        registro.FechaModifica = DateTime.Now;
                        registro.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
                    }

                    // Guardar los cambios en el contexto
                    context.EsquemaVistaColumna.UpdateRange(registros);
                }

                return context.SaveChanges() >= 0;
            });
        }

        public bool UpdateEsquemaValidacion(EsquemaVista newRecord)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdEsquemaVista == newRecord.IdEsquemaVista);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

                context.EsquemaVista.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
        public List<EsquemaVistaOnaDto> GetListaEsquemaByOna(int idONA)
        {
            return ExecuteDbOperation(context =>
                (from v in context.EsquemaVista
                 join e in context.Esquema on v.IdEsquema equals e.IdEsquema
                 where v.IdONA == idONA && v.Estado == "A"
                 select new EsquemaVistaOnaDto
                 {
                     IdEsquemaVista = v.IdEsquemaVista,
                     EsquemaVista = e.EsquemaVista,
                     VistaOrigen = v.VistaOrigen,
                     MostrarWeb = e.MostrarWeb,
                     IdEsquema = e.IdEsquema
                 }).ToList()
            );
        }
        public bool GuardarListaEsquemaVistaColumna(List<EsquemaVistaColumna> listaEsquemaVistaColumna)
        {
            // Validamos que la lista no esté vacía
            if (listaEsquemaVistaColumna == null || !listaEsquemaVistaColumna.Any())
            {
                return false;
            }

            // Asignamos valores comunes a cada elemento de la lista
            var userId = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            var fechaActual = DateTime.Now;

            foreach (var item in listaEsquemaVistaColumna)
            {
                item.IdUserCreacion = userId;
                item.IdUserModifica = userId;
                item.FechaModifica = fechaActual;
                item.Estado = "A";
            }

            return ExecuteDbOperation(context =>
            {
                // Añadimos toda la lista al contexto
                context.EsquemaVistaColumna.AddRange(listaEsquemaVistaColumna);
                return context.SaveChanges() >= 0;
            });
        }
        
    }
}
