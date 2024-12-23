namespace ClientApp.Services.IService
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
    }
}