using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedApp.Data;

namespace DataAccess.Data
{
    public class DbContextFactory : IDbContextFactory
    {
        public DbContext CreateDbContext(string connectionString, DatabaseType databaseType)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

            switch (databaseType)
            {
                case DatabaseType.SQLSERVER:
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
                case DatabaseType.MYSQL:
                    optionsBuilder.UseMySQL(connectionString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, null);
            }

            return new DynamicDbContext(optionsBuilder.Options);
        }
    }
}