using Microsoft.EntityFrameworkCore;
using SharedApp.Models.Dtos;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Repositories
{
    /// <summary>
    /// Repositorio para acceder a los datos relacionados con catálogos, grillas, filtros, dimensiones, grupos, roles y puntos de acceso.
    /// Implementa la interfaz <see cref="ICatalogosRepository"/>.
    /// </summary>
    public class CatalogosRepository : BaseRepository, ICatalogosRepository
    {
        /// <summary>
        /// Constructor para inicializar el repositorio de catálogos.
        /// </summary>
        /// <param name="logger">Instancia de <see cref="ILogger{CatalogosRepository}"/> para el registro de logs.</param>
        /// <param name="sqlServerDbContextFactory">Fábrica para el contexto de base de datos SQL Server.</param>
        public CatalogosRepository(
          ILogger<CatalogosRepository> logger,
          ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
        }

        /// <inheritdoc />
        public List<VwGrilla> ObtenerVwGrilla()
        {
            return ExecuteDbOperation(context =>
              context.VwGrilla
                .AsNoTracking()
                .OrderBy(c => c.MostrarWebOrden)
                .ToList());
        }

        /// <inheritdoc />
        public List<VwFiltro> ObtenerVwFiltro()
        {
            return ExecuteDbOperation(context =>
              context.VwFiltro
                .AsNoTracking()
                .OrderBy(c => c.MostrarWebOrden)
                .ToList());
        }

        /// <inheritdoc />
        public List<VwDimension> ObtenerVwDimension()
        {
            return ExecuteDbOperation(context =>
              context.VwDimension
                .AsNoTracking()
                .OrderBy(c => c.MostrarWebOrden)
                .ToList());
        }

        /// <inheritdoc />
        public List<Homologacion> ObtenerGrupos()
        {
            return ExecuteDbOperation(context =>
              context.Homologacion
                .AsNoTracking()
                .Where(c => c.IdHomologacionGrupo == null)
                .OrderBy(c => c.MostrarWebOrden)
                .ToList());
        }

        /// <inheritdoc />
        public List<FnFiltroDetalleDto> ObtenerFiltroDetalles(string codigo)
        {
            return ExecuteDbOperation(context =>
              context.Database
                .SqlQuery<FnFiltroDetalleDto>($"SELECT * FROM fnFiltroDetalle({codigo})")
                .AsNoTracking()
                .OrderBy(c => c.MostrarWeb)
                .ToList());
        }

        /// <inheritdoc />
        public List<VwRol> ObtenerVwRol()
        {
            return ExecuteDbOperation(context =>
              context.VwRol
                .AsNoTracking()
                .OrderBy(c => c.Rol)
                .ToList());
        }

        /// <inheritdoc />
        public List<VwMenu> ObtenerVwMenu()
        {
            return ExecuteDbOperation(context =>
                context.VwMenu
                .AsNoTracking()
                .OrderBy(c => c.IdHomologacionMenu)
                .ToList());
        }

        public List<ONA> ObtenerOna()
        {
            return ExecuteDbOperation(context =>
                context.ONA
                .AsNoTracking()
                .OrderBy(c => c.IdONA)
                .ToList());
        }

    }
}
