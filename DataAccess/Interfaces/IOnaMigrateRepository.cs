using SharedApp.Dtos;

namespace DataAccess.Interfaces
{
    public interface IOnaMigrateRepository
    {
        List<OnaMigrateDto> postOnaMigrate(int idOna, int idEsquemaVista, string jsonParameter);
    }
}
