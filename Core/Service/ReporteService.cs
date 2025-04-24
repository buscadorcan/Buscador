using Core.Interfaces;
using DataAccess.Interfaces;
using DataAccess.Models;

namespace Core.Service
{
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository reporteRepository;

        public ReporteService(IReporteRepository reporteRepository)
        {
            this.reporteRepository = reporteRepository;
        }
        public VwHomologacion findByVista(string codigoHomologacion)
        {
            return  reporteRepository.findByVista(codigoHomologacion);
        }

        public List<VwAcreditacionEsquema> ObtenerVwAcreditacionEsquema()
        {
            return  reporteRepository.ObtenerVwAcreditacionEsquema();
        }

        public List<VwAcreditacionOna> ObtenerVwAcreditacionOna()
        {
            return  reporteRepository.ObtenerVwAcreditacionOna();
        }

        public List<VwActualizacionONA> ObtenerVwActualizacionONA()
        {
            return  reporteRepository.ObtenerVwActualizacionONA();
        }

        public List<VwBusquedaFecha> ObtenerVwBusquedaFecha()
        {
            return  reporteRepository.ObtenerVwBusquedaFecha();
        }

        public List<VwBusquedaFiltro> ObtenerVwBusquedaFiltro()
        {
            return  reporteRepository.ObtenerVwBusquedaFiltro();
        }

        public List<VwBusquedaUbicacion> ObtenerVwBusquedaUbicacion()
        {
            return  reporteRepository.ObtenerVwBusquedaUbicacion();
        }

        public List<VwCalificaUbicacion> ObtenerVwCalificaUbicacion()
        {
            return reporteRepository.ObtenerVwCalificaUbicacion();
        }

        public List<VwEsquemaPais> ObtenerVwEsquemaPais()
        {
            return  reporteRepository.ObtenerVwEsquemaPais();
        }

        public List<VwEstadoEsquema> ObtenerVwEstadoEsquema()
        {
            return  reporteRepository.ObtenerVwEstadoEsquema();
        }

        public List<VwOecFecha> ObtenerVwOecFecha()
        {
            return  reporteRepository.ObtenerVwOecFecha();
        }

        public List<VwOecPais> ObtenerVwOecPais()
        {
            return  reporteRepository.ObtenerVwOecPais();
        }

        public List<VwOrganismoActividad> ObtenerVwOrganismoActividad()
        {
            return  reporteRepository.ObtenerVwOrganismoActividad();
        }

        public List<VwOrganismoRegistrado> ObtenerVwOrganismoRegistrado()
        {
            return  reporteRepository.ObtenerVwOrganismoRegistrado();
        }

        public List<VwOrganizacionEsquema> ObtenerVwOrganizacionEsquema()
        {
            return  reporteRepository.ObtenerVwOrganizacionEsquema();
        }

        public List<VwProfesionalCalificado> ObtenerVwProfesionalCalificado()
        {
            return  reporteRepository.ObtenerVwProfesionalCalificado();
        }

        public List<VwProfesionalEsquema> ObtenerVwProfesionalEsquema()
        {
            return  reporteRepository.ObtenerVwProfesionalEsquema();
        }

        public List<VwProfesionalFecha> ObtenerVwProfesionalFecha()
        {
            return  reporteRepository.ObtenerVwProfesionalFecha();
        }

        public List<VwProfesionalOna> ObtenerVwProfesionalOna()
        {
            return  reporteRepository.ObtenerVwProfesionalOna();
        }
    }
}
