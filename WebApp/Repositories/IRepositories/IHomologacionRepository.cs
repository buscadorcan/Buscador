using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
    public interface IHomologacionRepository
    {
        bool Update(Homologacion data);
        bool Create(Homologacion data);
        Homologacion? FindById(int id);
        ICollection<Homologacion> FindByParent();
        List<Homologacion> FindByIds(int[] ids);
        /// <summary>
        /// Obtiene el Homologación por codigo homologación.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="VwHomologacion"/>.</returns>
        List<VwHomologacion> ObtenerVwHomologacionPorCodigo(string codigoHomologacion);
    }
}
