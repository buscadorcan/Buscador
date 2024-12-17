using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaRepository
  {
    bool Update(Esquema data);
    bool Create(Esquema data);
    Esquema? FindById(int Id);
    Esquema? FindByViewName(string esquemaVista);
    List<Esquema> FindAll();
    List<Esquema> FindAllWithViews();
  }
}