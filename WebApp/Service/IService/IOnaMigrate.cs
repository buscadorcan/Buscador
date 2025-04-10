using SharedApp.Models.Dtos;

namespace WebApp.Service.IService
{
    public interface IOnaMigrate
    {
       Task< List<OnaMigrateDto>> postOnaMigrate(string view, int idOna, int idEsquema);
    }
}
