using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface ICatalogosRepository
    {
        /// <summary>
        /// WebApp/ObtenerVwGrilla: Obtiene el esquema de la grilla.
        /// Este método permite recuperar la estructura y configuración de la grilla de datos utilizada en el sistema.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos VwGrilla con la información de la grilla.
        /// </returns>
        List<VwGrilla> ObtenerVwGrilla();

        /// <summary>
        /// WebApp/ObtenerVwFiltro: Obtiene el esquema de los filtros.
        /// Este método permite recuperar la configuración de los filtros disponibles en el sistema.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos VwFiltro con la estructura de los filtros configurados.
        /// </returns>
        List<VwFiltro> ObtenerVwFiltro();

        /// <summary>
        /// WebApp/ObtenerVwDimension: Obtiene el esquema de las dimensiones.
        /// Este método permite obtener la estructura de dimensiones utilizadas en la aplicación.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos VwDimension con la información de las dimensiones registradas.
        /// </returns>
        List<VwDimension> ObtenerVwDimension();

        /// <summary>
        /// WebApp/ObtenerGrupos: Obtiene los grupos de homologación.
        /// Este método permite recuperar los diferentes grupos de homologación definidos en el sistema.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos Homologacion con la información de los grupos de homologación existentes.
        /// </returns>
        List<Homologacion> ObtenerGrupos();

        /// <summary>
        /// WebApp/FindVwHGByCode: Busca un grupo de homologación por su código de homologación.
        /// Este método permite obtener la información de un grupo de homologación específico a partir de su código.
        /// </summary>
        /// <param name="codigoHomologacion">Código único del grupo de homologación.</param>
        /// <returns>
        /// Devuelve un objeto VwHomologacionGrupo con la información del grupo de homologación encontrado.
        /// Si el código no existe, devuelve un valor nulo.
        /// </returns>
        VwHomologacionGrupo? FindVwHGByCode(string codigoHomologacion);

        /// <summary>
        /// WebApp/ObtenerFiltroDetalles: Obtiene los detalles de un filtro específico.
        /// Este método permite recuperar información detallada de un filtro en función de su código identificador.
        /// </summary>
        /// <param name="codigo">Código único del filtro a consultar.</param>
        /// <returns>
        /// Devuelve una lista de objetos vwFiltroDetalle con los detalles del filtro solicitado.
        /// </returns>
        List<vwFiltroDetalle> ObtenerFiltroDetalles(string codigo);

        /// <summary>
        /// WebApp/ObtenerVwRol: Obtiene el esquema de roles.
        /// Este método permite recuperar la lista de roles disponibles en el sistema.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos VwRol con la información de los roles existentes.
        /// </returns>
        List<VwRol> ObtenerVwRol();

        /// <summary>
        /// WebApp/FindVwRolByHId: Busca un rol en el esquema de homologación por su identificador.
        /// Este método permite obtener información detallada de un rol específico dentro del sistema de homologación.
        /// </summary>
        /// <param name="idHomologacionRol">Identificador único del rol dentro del esquema de homologación.</param>
        /// <returns>
        /// Devuelve un objeto VwRol con la información del rol solicitado.
        /// </returns>
        VwRol FindVwRolByHId(int idHomologacionRol);

        /// <summary>
        /// WebApp/ObtenerVwMenu: Obtiene el esquema del menú.
        /// Este método permite recuperar la estructura y configuración del menú del sistema.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos VwMenu con la información del menú y sus elementos.
        /// </returns>
        List<VwMenu> ObtenerVwMenu();

        /// <summary>
        /// WebApp/ObtenerOna: Obtiene el esquema de ONAs.
        /// Este método permite recuperar la lista de Organismos Nacionales de Acreditación (ONAs) registrados en el sistema.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos ONA con la información de los ONAs disponibles.
        /// </returns>
        List<ONA> ObtenerOna();

        /// <summary>
        /// WebApp/ObtenervwOna: Obtiene información detallada de los ONAs.
        /// Este método permite obtener información ampliada sobre los Organismos Nacionales de Acreditación registrados.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos vwONA con la información detallada de los ONAs.
        /// </returns>
        List<vwONA> ObtenervwOna();

        /// <summary>
        /// WebApp/ObtenerVwHomologacionGrupo: Obtiene la lista de grupos de homologación.
        /// Este método permite recuperar la lista de grupos de homologación existentes en el sistema.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos VwHomologacionGrupo con la información de los grupos de homologación.
        /// </returns>
        List<VwHomologacionGrupo> ObtenerVwHomologacionGrupo();

        /// <summary>
        /// WebApp/ObtenerVwPanelOna: Obtiene información del panel de ONAs.
        /// Este método permite recuperar la información consolidada de los Organismos Nacionales de Acreditación (ONAs)
        /// en un formato estructurado dentro del panel de visualización.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos vwPanelONA con la información detallada de los ONAs disponibles en el panel.
        /// </returns>
        List<vwPanelONA> ObtenerVwPanelOna();

        /// <summary>
        /// WebApp/ObtenervwEsquemaOrganiza: Obtiene la estructura organizativa del esquema.
        /// Este método permite recuperar la estructura organizativa de un esquema dentro del sistema,
        /// mostrando las relaciones jerárquicas y su distribución.
        /// </summary>
        /// <returns>
        /// Devuelve una lista de objetos vwEsquemaOrganiza con la información estructurada de la organización del esquema.
        /// </returns>
        List<vwEsquemaOrganiza> ObtenervwEsquemaOrganiza();


    }
}
