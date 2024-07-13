using Microsoft.EntityFrameworkCore;

namespace WebApp.Service
{
    public class DynamicDbContext : DbContext
    {
        public DynamicDbContext(DbContextOptions<DbContext> options) : base(options) { }
    }
}
