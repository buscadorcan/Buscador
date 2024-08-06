using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
  public interface IOrganizacionDataRepository
  {

    bool Update(OrganizacionData data);
    OrganizacionData? Create(OrganizacionData data);
    OrganizacionData? FindById(int Id);
    ICollection<OrganizacionData> FindAll();
    int GetLastId();
    bool DeleteOldRecords(int IdHomologacionEsquema, int IdConexion);
    bool DeleteOldRecord(string idVista, string idOrganizacion, int IdConexion, int idHomologacionEsquema);
    bool DeleteByExcludingVistaIds(List<string> idsVista, string idOrganizacion, int idConexion, int idOrganizacionData);
  }
}