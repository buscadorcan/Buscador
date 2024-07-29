using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
  public interface IDataLakeOrganizacionRepository
  {

    bool Update(DataLakeOrganizacion data);
    DataLakeOrganizacion? Create(DataLakeOrganizacion data);
    DataLakeOrganizacion? FindById(int Id);
    ICollection<DataLakeOrganizacion> FindAll();
    int GetLastId();
    bool DeleteOldRecords(int IdHomologacionEsquema);
    bool DeleteOldRecord(string idVista, string idOrganizacion, List<int> dataLakeIds);
    bool DeleteByExcludingVistaIds(List<string> idsVista, string idOrganizacion, List<int> dataLakeIds, int idDataLakeOrganizacion);
  }
}