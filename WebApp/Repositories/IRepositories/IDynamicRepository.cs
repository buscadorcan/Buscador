using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
  public interface IDynamicRepository
  {
    List<PropiedadesTablaDto> GetProperties(int idONA, string viewName);
    List<string> GetViewNames(int idONA);
    List<EsquemaVistaDto> GetListaValidacionEsquema(int idONA, int idEsquemaVista);
    }
}
