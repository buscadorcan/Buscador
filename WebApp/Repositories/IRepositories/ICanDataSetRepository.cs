using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
  public interface ICanDataSetRepository
  {

    bool Update(CanDataSet data);
    CanDataSet? Create(CanDataSet data);
    CanDataSet? FindById(int Id);
    ICollection<CanDataSet> FindAll();
    int GetLastId();
    bool DeleteOldRecords(int idHomologacionEsquema, int idConexion);
    bool DeleteOldRecord(string idVista, string idEnte, int idConexion, int idHomologacionEsquema);
    bool DeleteByExcludingVistaIds(List<string> idsVista, string idEnte, int idConexion, int idCanDataSet);
  }
}