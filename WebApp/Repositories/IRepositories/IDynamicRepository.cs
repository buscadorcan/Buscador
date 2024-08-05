using SharedApp.Models.Dtos;

namespace WebApp.Repositories.IRepositories
{
    public interface IDynamicRepository
    {
        List<PropiedadesTablaDto> GetProperties(string codigoHomologacion, string viewName);
        List<string> GetViewNames(string codigoHomologacion);
    }
}