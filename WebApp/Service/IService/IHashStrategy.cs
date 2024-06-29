namespace WebApp.Service.IService
{
    public interface IHashStrategy
    {
        string ComputeHash(string? input);
    }
}
