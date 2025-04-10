using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    public interface IOnaMigrateRepository
    {
        List<OnaMigrateDto> postOnaMigrate(int idOna, int idEsquemaVista, string jsonParameter);
    }
}
