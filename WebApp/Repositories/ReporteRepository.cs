using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class ReporteRepository : BaseRepository, IReporteRepository
    {
        private readonly IJwtService _jwtService;
        public ReporteRepository(
          IJwtService jwtService,
          ILogger<ReporteRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
        }

        public List<VwAcreditacionOna> ObtenerVwAcreditacionOna()
        {
            return ExecuteDbOperation(context =>
                context.VwAcreditacionOna
                    .AsNoTracking()
                    .ToList());
        }

        public List<VwAcreditacionEsquema> ObtenerVwAcreditacionEsquema()
        {
            return ExecuteDbOperation(context =>
                context.VwAcreditacionEsquema
                    .AsNoTracking()
                    .ToList());
        }

        public List<VwEstadoEsquema> ObtenerVwEstadoEsquema()
        {
            return ExecuteDbOperation(context =>
                context.VwEstadoEsquema
                    .AsNoTracking()
                    .ToList());
        }

        public List<VwOecPais> ObtenerVwOecPais()
        {
            return ExecuteDbOperation(context =>
                context.VwOecPais
                    .AsNoTracking()
                    .ToList());
        }

        public List<VwEsquemaPais> ObtenerVwEsquemaPais()
        {
            return ExecuteDbOperation(context =>
                context.VwEsquemaPais
                    .AsNoTracking()
                    .ToList());
        }

        public List<VwOecFecha> ObtenerVwOecFecha()
        {
            return ExecuteDbOperation(context =>
                context.VwOecFecha
                    .AsNoTracking()
                    .ToList());
        }
    }
}
