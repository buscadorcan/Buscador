using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaVistaRepository
  {
    bool Update(EsquemaVista data);
    bool Create(EsquemaVista data);
    EsquemaVista? FindById(int Id);
    EsquemaVista? FindByIdEsquema(int IdEsquema);
    List<EsquemaVista> FindAll();
  }
}