using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Service.IService
{
    public interface IDbContextFactory
    {
        DbContext CreateDbContext(string connectionString, DatabaseType databaseType);
    }
}
