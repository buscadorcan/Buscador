using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaDataRepository
  {
    bool Update(EsquemaData data);
    EsquemaData? Create(EsquemaData data);
    Task<EsquemaData?> CreateAsync(EsquemaData data);

    EsquemaData? FindById(int Id);
    ICollection<EsquemaData> FindAll();
    int GetLastId();
    bool DeleteOldRecords(int IdEsquemaVista);
    Task<bool> DeleteOldRecordsAsync(int IdEsquemaVista);
    bool DeleteOldRecord(string idVista, string idEnte, int idConexion, int idHomologacionEsquema);
    bool DeleteByExcludingVistaIds(List<string> idsVista, string idEnte, int idConexion, int idEsquemaData);
  }
}
