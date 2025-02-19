using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface ICatalogosRepository
    {
        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwGrilla: Obtiene el esquema de la grilla.
         */
        List<VwGrilla> ObtenerVwGrilla();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwFiltro: Obtiene el esquema de los filtros.
         */
        List<VwFiltro> ObtenerVwFiltro();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwDimension: Obtiene el esquema de las dimensiones.
         */
        List<VwDimension> ObtenerVwDimension();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerGrupos: Obtiene los grupos de homologación.
         */
        List<Homologacion> ObtenerGrupos();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindVwHGByCode: Busca un grupo de homologación por su código de homologación.
         */
        VwHomologacionGrupo? FindVwHGByCode(string codigoHomologacion);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerFiltroDetalles: Obtiene los detalles de un filtro específico.
         */
        List<vwFiltroDetalle> ObtenerFiltroDetalles(string codigo);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwRol: Obtiene el esquema de roles.
         */
        List<VwRol> ObtenerVwRol();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindVwRolByHId: Busca un rol en el esquema de homologación por su identificador.
         */
        VwRol FindVwRolByHId(int idHomologacionRol);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwMenu: Obtiene el esquema del menú.
         */
        List<VwMenu> ObtenerVwMenu();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerOna: Obtiene el esquema de ONAs.
         */
        List<ONA> ObtenerOna();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenervwOna: Obtiene información detallada de los ONAs.
         */
        List<vwONA> ObtenervwOna();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwHomologacionGrupo: Obtiene la lista de grupos de homologación.
         */
        List<VwHomologacionGrupo> ObtenerVwHomologacionGrupo();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwPanelOna: Obtiene información del panel de ONAs.
         */
        List<vwPanelONA> ObtenerVwPanelOna();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenervwEsquemaOrganiza: Obtiene la estructura organizativa del esquema.
         */
        List<vwEsquemaOrganiza> ObtenervwEsquemaOrganiza();

    }
}
