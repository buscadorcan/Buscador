using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaVistaColumnaRepository
  {
    bool Update(EsquemaVistaColumna data);
    bool Create(EsquemaVistaColumna data);
    EsquemaVistaColumna? FindById(int Id);
        List<EsquemaVistaColumna> FindByIdEsquemaVista(int IdEsquemaVista);
        List<EsquemaVistaColumna> _FindByIdEsquemaVista(int IdEsquemaVista, int IdOna);
    List<EsquemaVistaColumna> FindAll();
  }
}