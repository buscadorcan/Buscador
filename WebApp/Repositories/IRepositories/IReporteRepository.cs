using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IReporteRepository
    {
        /* 
 * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
 * WebApp/findByVista: Obtiene una homologación específica en base a su código de homologación.
 */
        VwHomologacion findByVista(string codigoHomologacion);

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwAcreditacionOna: Obtiene la lista de acreditaciones de ONAs.
         */
        List<VwAcreditacionOna> ObtenerVwAcreditacionOna();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwAcreditacionEsquema: Obtiene la lista de acreditaciones de esquemas.
         */
        List<VwAcreditacionEsquema> ObtenerVwAcreditacionEsquema();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwEstadoEsquema: Obtiene el estado de los esquemas registrados.
         */
        List<VwEstadoEsquema> ObtenerVwEstadoEsquema();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwOecPais: Obtiene información sobre OECs por país.
         */
        List<VwOecPais> ObtenerVwOecPais();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwEsquemaPais: Obtiene información sobre esquemas por país.
         */
        List<VwEsquemaPais> ObtenerVwEsquemaPais();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwOecFecha: Obtiene información sobre OECs organizados por fecha.
         */
        List<VwOecFecha> ObtenerVwOecFecha();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwProfesionalCalificado: Obtiene información sobre profesionales calificados.
         */
        List<VwProfesionalCalificado> ObtenerVwProfesionalCalificado();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwProfesionalOna: Obtiene información sobre profesionales en ONAs.
         */
        List<VwProfesionalOna> ObtenerVwProfesionalOna();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwProfesionalEsquema: Obtiene información sobre profesionales asociados a esquemas.
         */
        List<VwProfesionalEsquema> ObtenerVwProfesionalEsquema();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwProfesionalFecha: Obtiene información sobre profesionales organizados por fecha.
         */
        List<VwProfesionalFecha> ObtenerVwProfesionalFecha();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwCalificaUbicacion: Obtiene información sobre calificaciones por ubicación.
         */
        List<VwCalificaUbicacion> ObtenerVwCalificaUbicacion();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwBusquedaFecha: Obtiene información sobre búsquedas realizadas por fecha.
         */
        List<VwBusquedaFecha> ObtenerVwBusquedaFecha();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwBusquedaFiltro: Obtiene información sobre búsquedas realizadas por filtros aplicados.
         */
        List<VwBusquedaFiltro> ObtenerVwBusquedaFiltro();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwBusquedaUbicacion: Obtiene información sobre búsquedas realizadas por ubicación.
         */
        List<VwBusquedaUbicacion> ObtenerVwBusquedaUbicacion();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwActualizacionONA: Obtiene información sobre las actualizaciones de ONAs.
         */
        List<VwActualizacionONA> ObtenerVwActualizacionONA();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwOrganismoRegistrado: Obtiene información sobre organismos registrados.
         */
        List<VwOrganismoRegistrado> ObtenerVwOrganismoRegistrado();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwOrganizacionEsquema: Obtiene información sobre la relación entre organizaciones y esquemas.
         */
        List<VwOrganizacionEsquema> ObtenerVwOrganizacionEsquema();

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ObtenerVwOrganismoActividad: Obtiene información sobre actividades realizadas por organismos.
         */
        List<VwOrganismoActividad> ObtenerVwOrganismoActividad();


    }
}
