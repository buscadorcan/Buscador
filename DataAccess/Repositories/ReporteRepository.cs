using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class ReporteRepository : BaseRepository, IReporteRepository
    {
        public ReporteRepository(
          ILogger<ReporteRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
        }

        //titulos de reportes
        public VwHomologacion findByVista(string codigoHomologacion)
        {
            return ExecuteDbOperation(context =>
              context.VwHomologacion
                .AsNoTracking()
                .Where(c => c.CodigoHomologacion == codigoHomologacion)
                .FirstOrDefault());
        }

        //usuario
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

        //read
        public List<VwProfesionalCalificado> ObtenerVwProfesionalCalificado()
        {
            return ExecuteDbOperation(context =>
                context.VwProfesionalCalificado
                    .AsNoTracking()
                    .ToList());
        }
        public List<VwProfesionalOna> ObtenerVwProfesionalOna()
        {
            return ExecuteDbOperation(context =>
                context.VwProfesionalOna
                    .AsNoTracking()
                    .ToList());
        }
        public List<VwProfesionalEsquema> ObtenerVwProfesionalEsquema()
        {
            return ExecuteDbOperation(context =>
                context.VwProfesionalEsquema
                    .AsNoTracking()
                    .ToList());
        }
        public List<VwProfesionalFecha> ObtenerVwProfesionalFecha()
        {
            return ExecuteDbOperation(context =>
                context.VwProfesionalFecha
                    .AsNoTracking()
                    .ToList());
        }
        public List<VwCalificaUbicacion> ObtenerVwCalificaUbicacion()
        {
            return ExecuteDbOperation(context =>
                context.VwCalificaUbicacion
                    .AsNoTracking()
                    .ToList());
        }

        //can
        public List<VwBusquedaFecha> ObtenerVwBusquedaFecha()
        {
            return ExecuteDbOperation(context =>
                context.VwBusquedaFecha
                    .AsNoTracking()
                    .ToList());
        }
        public List<VwBusquedaFiltro> ObtenerVwBusquedaFiltro()
        {
            return ExecuteDbOperation(context =>
                context.VwBusquedaFiltro
                    .AsNoTracking()
                    .ToList());
        }
        public List<VwBusquedaUbicacion> ObtenerVwBusquedaUbicacion()
        {
            return ExecuteDbOperation(context =>
                context.VwBusquedaUbicacion
                    .AsNoTracking()
                    .ToList());
        }
        public List<VwActualizacionONA> ObtenerVwActualizacionONA()
        {
            return ExecuteDbOperation(context =>
                context.VwActualizacionONA
                    .AsNoTracking()
                    .ToList());
        }

        //ona
        public List<VwOrganismoRegistrado> ObtenerVwOrganismoRegistrado()
        {
            return ExecuteDbOperation(context =>
                context.VwOrganismoRegistrado
                    .AsNoTracking()
                    .ToList());
        }
        public List<VwOrganizacionEsquema> ObtenerVwOrganizacionEsquema()
        {
            return ExecuteDbOperation(context =>
                context.VwOrganizacionEsquema
                    .AsNoTracking()
                    .ToList());
        }
        public List<VwOrganismoActividad> ObtenerVwOrganismoActividad()
        {
            return ExecuteDbOperation(context =>
                context.VwOrganismoActividad
                    .AsNoTracking()
                    .ToList());
        }
    }
}
