using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
  public interface ICanDataSetRepository
  {

    bool Update(CanDataSet data);
    CanDataSet? Create(CanDataSet data);
    CanDataSet? FindById(int Id);
    ICollection<CanDataSet> FindAll();
    int GetLastId();
    bool DeleteOldRecords(int IdHomologacionEsquema, int IdConexion);
    bool DeleteOldRecord(string idVista, string idOrganizacion, int IdConexion, int idHomologacionEsquema);
    bool DeleteByExcludingVistaIds(List<string> idsVista, string idOrganizacion, int idConexion, int idCanDataSet);
  }
}