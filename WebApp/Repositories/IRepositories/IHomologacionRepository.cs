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
    }
}
