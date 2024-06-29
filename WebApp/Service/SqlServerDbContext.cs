using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Service
{
    public class SqlServerDbContext : DbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }
        // modulo de seguridad
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Models.Endpoint> Endpoint { get; set; }
        public DbSet<UsuarioEndpointPermiso> UsuarioEndpointPermiso { get; set; }
        // vistas
        public DbSet<VwGrilla> VwGrilla { get; set; }
        public DbSet<VwFiltro> VwFiltro { get; set; }
        public DbSet<VwDimension> VwDimension { get; set; }
        // modulo de busquedas
        public DbSet<Homologacion> Homologacion { get; set; }
        public DbSet<DataLakeOrganizacion> DataLakeOrganizacion { get; set; }
        public DbSet<HomologacionEsquema> HomologacionEsquema { get; set; }
        public DbSet<DataLake> DataLake { get; set; }
        public DbSet<OrganizacionFullText> OrganizacionFullText { get; set; }
    }
}