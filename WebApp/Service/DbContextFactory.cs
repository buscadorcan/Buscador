using Microsoft.EntityFrameworkCore;
using WebApp.Models;
using WebApp.Service.IService;

namespace WebApp.Service
{
    public class DbContextFactory : IDbContextFactory
    {
        public DbContext CreateDbContext(string connectionString, DatabaseType databaseType)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

            switch (databaseType)
            {
                case DatabaseType.SqlServer:
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
                // case DatabaseType.PostgreSql:
                //     optionsBuilder.UseNpgsql(connectionString);
                //     break;
                // case DatabaseType.MySql:
                //     optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
                //     break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }

            return new DynamicDbContext(optionsBuilder.Options);
        }
    }
}