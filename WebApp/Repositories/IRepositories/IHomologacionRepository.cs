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
        /// Obtiene el Homologaci�n por codigo homologaci�n.
        /// </summary>
        /// <returns>Una lista de objetos <see cref="VwHomologacion"/>.</returns>
        List<VwHomologacion> ObtenerVwHomologacionPorCodigo(string codigoHomologacion);
    }
}
