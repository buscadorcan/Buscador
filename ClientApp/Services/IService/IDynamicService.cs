using SharedApp.Models.Dtos;

namespace ClientApp.Services.IService {
    public interface IDynamicService
    {
        Task<List<PropiedadesTablaDto>> GetProperties(int idSystem, string viewName);
        Task<List<string>> GetViewNames(int idSystem);
    }
}   