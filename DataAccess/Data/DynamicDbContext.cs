using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data
{
    public class DynamicDbContext : DbContext
    {
        public DynamicDbContext(DbContextOptions<DbContext> options) : base(options) { }
    }
}
