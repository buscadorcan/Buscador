using Dapper;
using Microsoft.Data.SqlClient;
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
                .Where(c => c.IdHomologacionGrupo == null && c.Estado == "A")
                .OrderBy(c => c.MostrarWebOrden)
                .ToList());
        }
        /// <inheritdoc />
        public VwHomologacionGrupo? FindVwHGByCode(string codigoHomologacion)
        {
          return ExecuteDbOperation(context =>
              context.VwHomologacionGrupo
                .AsNoTracking()
                .Where(c => c.CodigoHomologacion == codigoHomologacion)
                .FirstOrDefault());
        }
        /// <inheritdoc />
        public List<vwFiltroDetalle> ObtenerFiltroDetalles(string codigo)
        {
            return ExecuteDbOperation(context =>
             context.vwFiltroDetalle
               .AsNoTracking()
               .Where(c => c.CodigoHomologacion == codigo)
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
        public VwRol FindVwRolByHId(int idHomologacionRol)
        {
            return ExecuteDbOperation(context =>
              context.VwRol
                .AsNoTracking()
                .FirstOrDefault(c => c.IdHomologacionRol == idHomologacionRol));
        }
        /// <inheritdoc />
        public List<VwHomologacionGrupo> ObtenerVwHomologacionGrupo()
        {
            return ExecuteDbOperation(context =>
              context.VwHomologacionGrupo
                .AsNoTracking()
                .OrderBy(c => c.MostrarWebOrden)
                .ToList());
        }
        /// <inheritdoc />
        public List<VwMenu> ObtenerVwMenu()
        {
            return ExecuteDbOperation(context =>
                context.VwMenu
                .AsNoTracking()
                //.OrderBy(c => c.IdHomologacionMenu)
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

        public List<vwPanelONA> ObtenerVwPanelOna()
        {
            return ExecuteDbOperation(context =>
              context.vwPanelONA
                .AsNoTracking()
                .OrderBy(c => c.NroOrg)
                .ToList());
        }

        public List<vwONA> ObtenervwOna()
        {
            return ExecuteDbOperation(context =>
                context.vwONA
                .AsNoTracking()
                .OrderBy(c => c.IdONA)
                .ToList());
        }

        public List<vwEsquemaOrganiza> ObtenervwEsquemaOrganiza()
        {
            return ExecuteDbOperation(context =>
                context.vwEsquemaOrganiza
                .AsNoTracking()
                .OrderBy(c => c.ONAPais)
                .ToList());
        }

    }
}
