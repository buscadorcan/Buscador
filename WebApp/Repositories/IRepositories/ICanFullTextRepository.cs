using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
  public interface ICanFullTextRepository
  {
    CanFullText Create(CanFullText data);
    CanFullText? FindById(int Id);
  }
}