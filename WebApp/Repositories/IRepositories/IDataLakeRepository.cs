using WebApp.Models;

namespace WebApp.Repositories.IRepositories {
  public interface IDataLakeRepository
  {

    DataLake? Update(DataLake data);
    DataLake Create(DataLake data);
    DataLake? FindById(int Id);
    DataLake? FindBy(DataLake dataLake);
    ICollection<DataLake> FindAll();
  }
}