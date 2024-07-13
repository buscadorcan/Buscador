namespace WebApp.Service.IService
{
    public interface ISqlServerDbContextFactory
    {
        SqlServerDbContext CreateDbContext();
    }
}
