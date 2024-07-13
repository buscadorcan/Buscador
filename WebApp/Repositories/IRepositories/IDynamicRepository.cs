using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    public interface IDynamicRepository
    {
        List<PropiedadesTablaDto> GetProperties(int idSystem, string viewName);
        List<string> GetViewNames(int idSystem);
    }
}