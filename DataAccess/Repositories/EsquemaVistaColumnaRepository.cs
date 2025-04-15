using System.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccess.Models;
using SharedApp.Services;

namespace DataAccess.Repositories
{
    public class EsquemaVistaColumnaRepository : BaseRepository, IEsquemaVistaColumnaRepository
    {
        private readonly IJwtService _jwtService;
        public EsquemaVistaColumnaRepository(
          IJwtService jwtService,
          ILogger<UsuarioRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
        }
        public bool Create(EsquemaVistaColumna data)
        {
            data.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            data.IdUserModifica = data.IdUserCreacion;

            return ExecuteDbOperation(context =>
            {
                context.EsquemaVistaColumna.Add(data);
                return context.SaveChanges() >= 0;
            });
        }
        public EsquemaVistaColumna? FindById(int id)
        {
            return ExecuteDbOperation(context => context.EsquemaVistaColumna.AsNoTracking().FirstOrDefault(u => u.IdEsquemaVistaColumna == id));
        }
        public List<EsquemaVistaColumna> FindByIdEsquemaVista(int IdEsquemaVista)
        {
            return ExecuteDbOperation(context => context.EsquemaVistaColumna.AsNoTracking().Where(u => u.IdEsquemaVista == IdEsquemaVista && u.Estado == "A").ToList());
        }
        public List<EsquemaVistaColumna> FindByIdEsquemaVistaOna(int idEsquemaVista, int idONA)
        {
            return ExecuteDbOperation(context =>
                (from vista in context.EsquemaVista
                 join columna in context.EsquemaVistaColumna
                 on vista.IdEsquemaVista equals columna.IdEsquemaVista
                 where vista.IdONA == idONA
                       && vista.Estado == "A"
                       && columna.Estado == "A"
                       && columna.IdEsquemaVista == idEsquemaVista
                 select new EsquemaVistaColumna
                 {
                     IdEsquemaVista = vista.IdEsquemaVista,
                     ColumnaVista = columna.ColumnaVista,
                     ColumnaEsquemaIdH = columna.ColumnaEsquemaIdH,
                     IdEsquemaVistaColumna = columna.IdEsquemaVistaColumna,
                     ColumnaEsquema = columna.ColumnaEsquema,
                     ColumnaVistaPK = columna.ColumnaVistaPK,

                 }).ToList());
        }
        public async Task<List<EsquemaVistaColumna>> FindByIdEsquemaVistaOnaAsync(int idEsquemaVista, int idONA)
        {
            return await ExecuteDbOperation(async context =>
                await (from vista in context.EsquemaVista
                       join columna in context.EsquemaVistaColumna
                       on vista.IdEsquemaVista equals columna.IdEsquemaVista
                       where vista.IdONA == idONA
                             && vista.Estado == "A"
                             && columna.Estado == "A"
                             && columna.IdEsquemaVista == idEsquemaVista
                       select new EsquemaVistaColumna
                       {
                           IdEsquemaVista = vista.IdEsquemaVista,
                           ColumnaVista = columna.ColumnaVista,
                           ColumnaEsquemaIdH = columna.ColumnaEsquemaIdH,
                           IdEsquemaVistaColumna = columna.IdEsquemaVistaColumna,
                           ColumnaEsquema = columna.ColumnaEsquema,
                           ColumnaVistaPK = columna.ColumnaVistaPK,
                       }).ToListAsync());
        }

        public List<EsquemaVistaColumna> FindAll()
        {
            return ExecuteDbOperation(context => context.EsquemaVistaColumna.AsNoTracking().Where(c => c.Estado.Equals("A")).ToList());
        }
        public bool Update(EsquemaVistaColumna newRecord)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, newRecord, u => u.IdEsquemaVistaColumna == newRecord.IdEsquemaVistaColumna);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

                context.EsquemaVistaColumna.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }

    }
}
