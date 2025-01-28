using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IReporteRepository
    {
        //titulos
        VwHomologacion findByVista(string codigoHomologacion);

        //usuario
        List<VwAcreditacionOna> ObtenerVwAcreditacionOna();
        List<VwAcreditacionEsquema> ObtenerVwAcreditacionEsquema();
        List<VwEstadoEsquema> ObtenerVwEstadoEsquema();
        List<VwOecPais> ObtenerVwOecPais();
        List<VwEsquemaPais> ObtenerVwEsquemaPais();
        List<VwOecFecha> ObtenerVwOecFecha();

        //read
        List<VwProfesionalCalificado> ObtenerVwProfesionalCalificado();
        List<VwProfesionalOna> ObtenerVwProfesionalOna();
        List<VwProfesionalEsquema> ObtenerVwProfesionalEsquema();
        List<VwProfesionalFecha> ObtenerVwProfesionalFecha();
        List<VwCalificaUbicacion> ObtenerVwCalificaUbicacion();

        //can
        List<VwBusquedaFecha> ObtenerVwBusquedaFecha();
        List<VwBusquedaFiltro> ObtenerVwBusquedaFiltro();
        List<VwBusquedaUbicacion> ObtenerVwBusquedaUbicacion();
        List<VwActualizacionONA> ObtenerVwActualizacionONA();

        //ona
        List<VwOrganismoRegistrado> ObtenerVwOrganismoRegistrado();
        List<VwOrganizacionEsquema> ObtenerVwOrganizacionEsquema();
        List<VwOrganismoActividad> ObtenerVwOrganismoActividad();

    }
}
