using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
  public interface IHomologacionEsquemaRepository
  {

    bool Update(HomologacionEsquema data);
    bool Create(HomologacionEsquema data);
    HomologacionEsquema? FindById(int Id);
    ICollection<HomologacionEsquema> FindAll();
  }
}