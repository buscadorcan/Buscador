using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaVistaRepository
  {
    bool Update(EsquemaVista data);
    bool Create(EsquemaVista data);
    EsquemaVista? FindById(int Id);
    EsquemaVista? FindByIdEsquema(int IdEsquema);
    EsquemaVista? _FindByIdEsquema(int IdEsquema, int idOna);
    Task<EsquemaVista?> _FindByIdEsquemaAsync(int IdEsquema, int idOna);
    List<EsquemaVista> FindAll();
  }
}