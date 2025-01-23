using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IReporteRepository
    {
        List<VwAcreditacionOna> ObtenerVwAcreditacionOna();
        List<VwAcreditacionEsquema> ObtenerVwAcreditacionEsquema();
        List<VwEstadoEsquema> ObtenerVwEstadoEsquema();
        List<VwOecPais> ObtenerVwOecPais();
        List<VwEsquemaPais> ObtenerVwEsquemaPais();
        List<VwOecFecha> ObtenerVwOecFecha();
    }
}
