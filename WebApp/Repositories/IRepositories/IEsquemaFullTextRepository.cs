using WebApp.Models;

namespace WebApp.Repositories.IRepositories
{
  public interface IEsquemaFullTextRepository
  {
    EsquemaFullText Create(EsquemaFullText data);
    EsquemaFullText? FindById(int Id);
  }
}
