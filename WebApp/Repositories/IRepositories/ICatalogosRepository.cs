using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories.IRepositories 
{
    public interface ICatalogosRepository
    {
        List<VwGrilla> ObtenerEtiquetaGrilla();
        List<VwFiltro> ObtenerEtiquetaFiltros();
        List<VwDimension> ObtenerDimension();
        List<Homologacion> ObtenerGrupos();
        List<FnFiltroDetalleDto> ObtenerFiltroDetalles(int IdHomologacion);
    }
}