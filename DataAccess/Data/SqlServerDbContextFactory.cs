using Microsoft.EntityFrameworkCore;
using DataAccess.Data;
using DataAccess.Interfaces;

public class SqlServerDbContextFactory(DbContextOptions<SqlServerDbContext> options) : ISqlServerDbContextFactory
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