using SharedApp.Models.Dtos;
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
    List<Esquema> GetListaEsquemaByOna(int idONA);
    bool UpdateEsquemaValidacion(EsquemaVista data);
    bool CreateEsquemaValidacion(EsquemaVista data);
    bool EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(int id);
    //EsquemaVistaColumna? GetEsquemaVistaColumnaByIdEquemaVistaAsync(int Id);
    EsquemaVistaColumna? GetEsquemaVistaColumnaByIdEquemaVistaAsync(int idOna, int idEsquema);    
    bool GuardarListaEsquemaVistaColumna(List<EsquemaVistaColumna> listaEsquemaVistaColumna, int? idOna, int? intidEsquema);

    }
}