using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    public class CatalogosRepository : BaseRepository, ICatalogosRepository
    {
        public CatalogosRepository(
            IDbContextFactory dbContextFactory,
            ILogger<CatalogosRepository> logger
        ) : base(dbContextFactory, logger)
        {
        }
        public ICollection<VwGrilla> ObtenerEtiquetaGrilla()
        {
            return ExecuteDbOperation(context => context.VwGrilla.OrderBy(c => c.MostrarWebOrden).ToList());
        }

        public ICollection<VwFiltro> ObtenerEtiquetaFiltros()
        {
            return ExecuteDbOperation(context => context.VwFiltro.OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public ICollection<VwDimension> ObtenerDimension()
        {
            return ExecuteDbOperation(context => context.VwDimension.OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public ICollection<Homologacion> ObtenerGrupos()
        {
            return ExecuteDbOperation(context => context.Homologacion.Where(c => c.IdHomologacionGrupo == null).OrderBy(c => c.MostrarWebOrden).ToList());
        }
        public ICollection<IVwHomologacion> ObtenerFiltroDetalles(int IdHomologacion)
        {
            return ExecuteDbOperation(context => context.Database.SqlQuery<IVwHomologacion>($"SELECT * FROM fnFiltroDetalle({IdHomologacion})").OrderBy( c => c.MostrarWebOrden).ToList());
        }
    }
}