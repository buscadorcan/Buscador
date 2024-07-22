using Microsoft.EntityFrameworkCore;
using SharedApp.Models.Dtos;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class CatalogosRepository : BaseRepository, ICatalogosRepository
    {
        public CatalogosRepository(
            ILogger<CatalogosRepository> logger,
            ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
        }
        public List<VwGrilla> ObtenerEtiquetaGrilla()
        {
            return ExecuteDbOperation(context => context.VwGrilla.OrderBy(c => c.MostrarWebOrden).ToList());
        }

        public List<VwFiltro> ObtenerEtiquetaFiltros()
        {
            return ExecuteDbOperation(context => context.VwFiltro.OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public List<VwDimension> ObtenerDimension()
        {
            return ExecuteDbOperation(context => context.VwDimension.OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public List<Homologacion> ObtenerGrupos()
        {
            return ExecuteDbOperation(context => context.Homologacion.Where(c => c.IdHomologacionGrupo == null).OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public List<FnFiltroDetalleDto> ObtenerFiltroDetalles(int IdHomologacion)
        {
            return ExecuteDbOperation(context => context.Database.SqlQuery<FnFiltroDetalleDto>($"SELECT * FROM fnFiltroDetalle({IdHomologacion})").OrderBy( c => c.MostrarWeb).ToList());
        }
    }
}