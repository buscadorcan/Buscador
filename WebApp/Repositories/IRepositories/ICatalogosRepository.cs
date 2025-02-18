using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    /// <summary>
    /// Interfaz para el repositorio de catálogos. 
    /// Proporciona métodos para obtener datos relacionados con grillas, filtros, dimensiones, grupos, roles y puntos de acceso.
    /// </summary>
    public interface ICatalogosRepository
    {
        /// <summary>
        /// Obtiene el esquema de la grilla.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="VwGrilla"/>.</returns>
        List<VwGrilla> ObtenerVwGrilla();

        /// <summary>
        /// Obtiene el esquema de los filtros.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="VwFiltro"/>.</returns>
        List<VwFiltro> ObtenerVwFiltro();

        /// <summary>
        /// Obtiene el esquema de las dimensiones.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="VwDimension"/>.</returns>
        List<VwDimension> ObtenerVwDimension();

        /// <summary>
        /// Obtiene los grupos de homologación.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="Homologacion"/>.</returns>
        List<Homologacion> ObtenerGrupos();

        /// <summary>
        /// Finds a <see cref="VwHomologacionGrupo"/> entity by its homologation code.
        /// </summary>
        /// <param name="codigoHomologacion">The homologation code used to search for the group.</param>
        /// <returns>
        /// A <see cref="VwHomologacionGrupo"/> object representing the group if found; otherwise, <c>null</c>.
        /// </returns>
        VwHomologacionGrupo? FindVwHGByCode(string codigoHomologacion);

        /// <summary>
        /// Obtiene los detalles de un filtro específico.
        /// </summary>
        /// <param name="IdHomologacion">El identificador del filtro de homologación.</param>
        /// <returns>Una lista de objetos <see cref="FnFiltroDetalleDto"/> con los detalles del filtro.</returns>
        List<vwFiltroDetalle> ObtenerFiltroDetalles(string codigo);

        /// <summary>
        /// Obtiene el esquema de roles.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="VwRol"/>.</returns>
        List<VwRol> ObtenerVwRol();

        /// <summary>
        /// Finds a <see cref="VwRol"/> entity by its associated homologation role ID.
        /// </summary>
        /// <param name="idHomologacionRol">The unique identifier of the homologation role to search for.</param>
        /// <returns>
        /// A <see cref="VwRol"/> object representing the role if found; otherwise, <c>null</c>.
        /// </returns>
        VwRol FindVwRolByHId(int idHomologacionRol);

        /// <summary>
        /// Obtiene el esquema de roles.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="VwMenu"/>.</returns>
        List<VwMenu> ObtenerVwMenu();
        /// <summary>
        /// Obtiene el esquema de roles.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="ONA"/>.</returns>
        List<ONA> ObtenerOna();
        /// <summary>
        /// Obtiene el esquema de roles.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="vwONA"/>.</returns>
        List<vwONA> ObtenervwOna();
        /// <summary>
        /// Obtiene el Homologación grupos.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="VwRol"/>.</returns>
        List<VwHomologacionGrupo> ObtenerVwHomologacionGrupo();
        /// <summary>
        /// Obtiene el Homologación grupos.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="vwPanelONA"/>.</returns>
        List<vwPanelONA> ObtenerVwPanelOna();
        /// <summary>
        /// Obtiene el Homologación grupos.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="vwEsquemaOrganiza"/>.</returns>

        List<vwEsquemaOrganiza> ObtenervwEsquemaOrganiza();
    }
}
