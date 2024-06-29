using WebApp.Models;

namespace WebApp.Repositories.IRepositories 
{
    public interface ICatalogosRepository
    {
        ICollection<VwGrilla> ObtenerEtiquetaGrilla();
        ICollection<VwFiltro> ObtenerEtiquetaFiltros();
        ICollection<VwDimension> ObtenerDimension();
        ICollection<Homologacion> ObtenerGrupos();
        ICollection<IVwHomologacion> ObtenerFiltroDetalles(int IdHomologacion);
    }
}