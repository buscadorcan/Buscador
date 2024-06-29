namespace WebApp.Service.IService
{
    public interface IDbContextFactory
    {
        SqlServerDbContext CreateDbContext();
    }
}