namespace WebApp.Service.IService
{
    public interface IPasswordGenerationStrategy
    {
        string GeneratePassword(int length);
    }
}