using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IDynamicService
    {
        Task<List<PropiedadesTablaDto>> GetProperties(string codigoHomologacion, string viewName);
        Task<List<string>> GetViewNames(string codigoHomologacion);
    }
}   