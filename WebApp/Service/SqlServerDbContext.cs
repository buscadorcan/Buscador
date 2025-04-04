using Microsoft.EntityFrameworkCore;
using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Service
{
    /// <summary>
    /// Representa el contexto de base de datos SQL Server.
    /// Administra la conexión a la base de datos y las entidades mapeadas.
    /// </summary>
    public class SqlServerDbContext : DbContext
    {
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="SqlServerDbContext"/>.
        /// </summary>
        /// <param name="options">Opciones de configuración para el contexto de la base de datos.</param>
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options) { }

        /// <summary>Tabla que almacena la información de los esquemas.</summary>
        public DbSet<Esquema> Esquema { get; set; }

        /// <summary>Tabla que contiene los datos asociados a un esquema.</summary>
        public DbSet<EsquemaData> EsquemaData { get; set; }

        /// <summary>Tabla que almacena información de texto completo de los esquemas.</summary>
        public DbSet<EsquemaFullText> EsquemaFullText { get; set; }

        /// <summary>Vista que representa los esquemas registrados.</summary>
        public DbSet<EsquemaVista> EsquemaVista { get; set; }

        /// <summary>Vista que almacena las columnas asociadas a los esquemas.</summary>
        public DbSet<EsquemaVistaColumna> EsquemaVistaColumna { get; set; }

        /// <summary>Tabla de homologación para transformar datos entre sistemas.</summary>
        public DbSet<Homologacion> Homologacion { get; set; }
        public DbSet<MenuRol> MenuRol { get; set; }

        /// <summary>Tabla de logs que registra el proceso de migración.</summary>
        public DbSet<LogMigracion> LogMigracion { get; set; }

        /// <summary>Tabla con detalles adicionales del proceso de migración.</summary>
        public DbSet<LogMigracionDetalle> LogMigracionDetalle { get; set; }

        /// <summary>Tabla que registra los scripts ejecutados en la base de datos.</summary>
        public DbSet<LogScript> LogScript { get; set; }

        /// <summary>Tabla de migraciones que provienen de archivos Excel.</summary>
        public DbSet<MigracionExcel> MigracionExcel { get; set; }

        /// <summary>Tabla principal que representa las conexiones ONA.</summary>
        public DbSet<ONA> ONA { get; set; }
        public DbSet<Menus> Menus { get; set; }

        /// <summary>Tabla que almacena las configuraciones de conexiones ONA.</summary>
        public DbSet<ONAConexion> ONAConexion { get; set; }

        /// <summary>Tabla que almacena información de los usuarios.</summary>
        public DbSet<Usuario> Usuario { get; set; }

        /// <summary>Tabla que registra los endpoints asignados a los usuarios.</summary>
        public DbSet<UsuarioEndpoint> UsuarioEndpoint { get; set; }

        /// <summary>Vista que representa dimensiones de los datos disponibles.</summary>
        public DbSet<VwDimension> VwDimension { get; set; }

        /// <summary>Vista que almacena filtros para realizar consultas específicas.</summary>
        public DbSet<VwFiltro> VwFiltro { get; set; }

        /// <summary>Vista que representa grillas o cuadrículas de información.</summary>
        public DbSet<VwGrilla> VwGrilla { get; set; }

        /// <summary>Vista que almacena los roles disponibles en el sistema.</summary>
        public DbSet<VwRol> VwRol { get; set; }

        /// <summary>Vista que almacena los roles disponibles en el sistema.</summary>
        public DbSet<VwPais> VwPais { get; set; }

        /// <summary>Vista que almacena los roles disponibles en el sistema.</summary>
        public DbSet<VwMenu> VwMenu { get; set; }

        /// <summary>Vista que almacena los roles disponibles en el sistema.</summary>
        public DbSet<vwONA> vwONA { get; set; }

        /// <summary>Vista que almacena homologacion grupos disponibles en el sistema.</summary>
        public DbSet<VwHomologacionGrupo> VwHomologacionGrupo { get; set; }
        public DbSet<VwHomologacion> VwHomologacion { get; set; }

        //usuario
        public DbSet<VwAcreditacionOna> VwAcreditacionOna { get; set; }
        public DbSet<VwAcreditacionEsquema> VwAcreditacionEsquema { get; set; }
        public DbSet<VwEstadoEsquema> VwEstadoEsquema { get; set; }
        public DbSet<VwOecPais> VwOecPais { get; set; }
        public DbSet<VwEsquemaPais> VwEsquemaPais { get; set; }
        public DbSet<VwOecFecha> VwOecFecha { get; set; }

        //read
        public DbSet<VwProfesionalCalificado> VwProfesionalCalificado { get; set; }
        public DbSet<VwProfesionalOna> VwProfesionalOna { get; set; }
        public DbSet<VwProfesionalEsquema> VwProfesionalEsquema { get; set; }
        public DbSet<VwProfesionalFecha> VwProfesionalFecha { get; set; }
        public DbSet<VwCalificaUbicacion> VwCalificaUbicacion { get; set; }

        //can
        public DbSet<VwBusquedaFecha> VwBusquedaFecha { get; set; }
        public DbSet<VwBusquedaFiltro> VwBusquedaFiltro { get; set; }
        public DbSet<VwBusquedaUbicacion> VwBusquedaUbicacion { get; set; }
        public DbSet<VwActualizacionONA> VwActualizacionONA { get; set; }

        //ona
        public DbSet<VwOrganismoRegistrado> VwOrganismoRegistrado { get; set; }
        public DbSet<VwOrganizacionEsquema> VwOrganizacionEsquema { get; set; }
        public DbSet<VwOrganismoActividad> VwOrganismoActividad { get; set; }

        public DbSet<vwFiltroDetalle> vwFiltroDetalle { get; set; }

        public DbSet<vwPanelONA> vwPanelONA { get; set; }
        public DbSet<vwEsquemaOrganiza> vwEsquemaOrganiza { get; set; }

        public DbSet<EventTracking> EventTracking { get; set; }

        public DbSet<VwEventUserAll> VwEventUserAll { get; set; }

        public DbSet<EventUser> EventUser { get; set; }

        public DbSet<PaginasMasVisitadaDto> EventPagMasVisit { get; set; }

        public DbSet<FiltrosMasUsadoDto> EventFiltroMasUsado { get; set; }

        public DbSet<VwEventTrackingSessionDto> EventSession { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VwMenu>().HasNoKey().ToView("vwMenu");

            //usuario
            modelBuilder.Entity<VwAcreditacionOna>().HasNoKey().ToView("vw_AcreditacionOna");
            modelBuilder.Entity<VwAcreditacionEsquema>().HasNoKey().ToView("vw_AcreditacionEsquema");
            modelBuilder.Entity<VwEstadoEsquema>().HasNoKey().ToView("vw_EstadoEsquema");
            modelBuilder.Entity<VwOecPais>().HasNoKey().ToView("vw_OecPais");
            modelBuilder.Entity<VwEsquemaPais>().HasNoKey().ToView("vw_EsquemaPais");
            modelBuilder.Entity<VwOecFecha>().HasNoKey().ToView("vw_OecFecha");
            modelBuilder.Entity<vwPanelONA>().HasNoKey().ToView("vwPanelONA");
            modelBuilder.Entity<vwEsquemaOrganiza>().HasNoKey().ToView("vwEsquemaOrganiza");

            //read
            modelBuilder.Entity<VwProfesionalCalificado>().HasNoKey().ToView("vw_ProfesionalCalificado");
            modelBuilder.Entity<VwProfesionalOna>().HasNoKey().ToView("vw_ProfesionalOna");
            modelBuilder.Entity<VwProfesionalEsquema>().HasNoKey().ToView("vw_ProfesionalEsquema");
            modelBuilder.Entity<VwProfesionalFecha>().HasNoKey().ToView("vw_ProfesionalFecha");
            modelBuilder.Entity<VwCalificaUbicacion>().HasNoKey().ToView("vw_CalificaUbicacion");

            //can
            modelBuilder.Entity<VwBusquedaFecha>().HasNoKey().ToView("vw_BusquedaFecha");
            modelBuilder.Entity<VwBusquedaFiltro>().HasNoKey().ToView("vw_BusquedaFiltro");
            modelBuilder.Entity<VwBusquedaUbicacion>().HasNoKey().ToView("vw_BusquedaUbicacion");
            modelBuilder.Entity<VwActualizacionONA>().HasNoKey().ToView("vw_ActualizacionONA");

            //ona
            modelBuilder.Entity<VwOrganismoRegistrado>().HasNoKey().ToView("vw_OrganismoRegistrado");
            modelBuilder.Entity<VwOrganizacionEsquema>().HasNoKey().ToView("vw_OrganizacionEsquema");
            modelBuilder.Entity<VwOrganismoActividad>().HasNoKey().ToView("vw_OrganismoActividad");
            modelBuilder.Entity<vwONA>().HasNoKey().ToView("vwONA");

            //Event
            modelBuilder.Entity<VwEventUserAll>().HasNoKey().ToView("vw_EventUserAll");
            modelBuilder.Entity<EventUser>().HasNoKey().ToView("vw_EventUserCAN");
            modelBuilder.Entity<EventUser>().HasNoKey().ToView("vw_EventUserONA");
            modelBuilder.Entity<EventUser>().HasNoKey().ToView("vw_EventUserREAD");
            modelBuilder.Entity<EventUser>().HasNoKey().ToView("vw_EventUserSEARCH");

            //Report Event
            modelBuilder.Entity<VwEventTrackingSessionDto>().HasNoKey().ToView("EXEC GetTiempoEnSession");
            modelBuilder.Entity<PaginasMasVisitadaDto>().HasNoKey().ToSqlQuery("EXEC getPaginasMasVisitada");
            modelBuilder.Entity<FiltrosMasUsadoDto>().HasNoKey().ToSqlQuery("EXEC GetFiltroMasUsado");
        }

    }
}
