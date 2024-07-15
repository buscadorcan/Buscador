using Microsoft.EntityFrameworkCore;
using SharedApp.Data;

namespace WebApp.Service.IService
{
    public interface IDbContextFactory
    {
        DbContext CreateDbContext(string connectionString, DatabaseType databaseType);
    }
}
