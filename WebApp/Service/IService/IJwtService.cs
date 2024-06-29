namespace WebApp.Service.IService
{
    public interface IJwtService
    {
        string GenerateJwtToken(int userId);
        int GetUserIdFromToken(string token);
        string? GetTokenFromHeader();
    }
}