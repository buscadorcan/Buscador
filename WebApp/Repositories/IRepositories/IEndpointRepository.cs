namespace WebApp.Repositories.IRepositories {
    public interface IEndpointRepository
    {
        Models.Endpoint? FindById(int idEndpoint);
        ICollection<Models.Endpoint> FindAll();
        bool Create(Models.Endpoint endpoint);
        bool IsUniqueUserUrl(string? nombre, string? url);
    }
}