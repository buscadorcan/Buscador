using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaDataRepository
  {
    bool Update(EsquemaData data);
    EsquemaData? Create(EsquemaData data);
    EsquemaData? FindById(int Id);
    ICollection<EsquemaData> FindAll();
    int GetLastId();
    bool DeleteOldRecords(int IdEsquemaVista);
    bool DeleteOldRecord(string idVista, string idEnte, int idConexion, int idHomologacionEsquema);
    bool DeleteByExcludingVistaIds(List<string> idsVista, string idEnte, int idConexion, int idEsquemaData);
  }
}
