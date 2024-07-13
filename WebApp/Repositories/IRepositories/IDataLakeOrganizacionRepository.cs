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
  }
}