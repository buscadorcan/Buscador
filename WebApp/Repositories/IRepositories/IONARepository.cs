using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IONARepository
  {
    bool Update(ONA data);
    bool Create(ONA data);
    ONA? FindById(int Id);
    ONA? FindBySiglas(string siglas);
    List<ONA> FindAll();
  }
}
