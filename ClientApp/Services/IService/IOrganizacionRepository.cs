// Utilizaremos el patrón Repository para esto.
namespace ClientApp.Services.IService {
    public interface IOrganizacionRepository
    {
        Task<List<Organizacion>> BuscarPalabraAsync(string campo, string value);
    }
}