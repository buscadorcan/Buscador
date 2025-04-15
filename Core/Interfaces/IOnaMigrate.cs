using SharedApp.Dtos;

namespace Core.Interfaces
{
    public interface IOnaMigrate
    {
       Task< List<OnaMigrateDto>> postOnaMigrate(string view, int idOna, int idEsquema);
    }
}
