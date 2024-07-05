using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
  public interface IOrganizacionFullTextRepository
  {
    OrganizacionFullText Create(OrganizacionFullText data);
    OrganizacionFullText? FindById(int Id);
  }
}