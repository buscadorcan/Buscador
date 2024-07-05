using Microsoft.EntityFrameworkCore;
using WebApp.Service.IService;
using WebApp.Service;

public class SqlServerDbContextFactory(DbContextOptions<SqlServerDbContext> options) : IDbContextFactory
{
    private readonly DbContextOptions<SqlServerDbContext> _options = options;
    public SqlServerDbContext CreateDbContext()
    {
        var context = new SqlServerDbContext(_options);
        context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        context.ChangeTracker.LazyLoadingEnabled = false;

        return context;
    }
}