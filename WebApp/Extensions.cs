using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using DataAccess.Interfaces;
using Core.Service;
using Core.Interfaces;
using DataAccess.Repositories;
using DataAccess.Data;
using Core.Services;
using Core.Mappers;

namespace WebApp.Extensions
{
    /// <summary>
    /// Clase de extensiones para la configuración de servicios en la aplicación ASP.NET Core.
    /// Proporciona métodos para configurar los DbContext, servicios, autenticación y Swagger.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Configura los DbContexts y las fábricas de contextos para la aplicación.
        /// </summary>
        /// <param name="services">El contenedor de servicios de la aplicación.</param>
        /// <param name="configuration">La configuración de la aplicación.</param>
        public static void ConfigureDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            // Configura el DbContext principal utilizando SQL Server.
            services.AddDbContextPool<SqlServerDbContext>(options =>
              options.UseSqlServer(configuration.GetConnectionString("Mssql-CanDb"))
            );

            // Configura una fábrica para crear instancias del DbContext.
            services.AddScoped<ISqlServerDbContextFactory>(provider =>
            {
                var options = provider.GetRequiredService<DbContextOptions<SqlServerDbContext>>();
                return new SqlServerDbContextFactory(options);
            });

            // Registra una fábrica global para manejar diferentes contextos.
            services.AddSingleton<IDbContextFactory, DbContextFactory>();
        }

        /// <summary>
        /// Configura los servicios y repositorios de la aplicación.
        /// </summary>
        /// <param name="services">El contenedor de servicios de la aplicación.</param>
        public static void ConfigureServices(this IServiceCollection services)
        {

            services.AddAutoMapper(typeof(Mapper));

            // Scrutor: registra todas las interfaces y clases de Core y DataAccess
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(EmailService), typeof(ONARepository)) // Puedes poner clases de ejemplo de cada ensamblado
                .AddClasses(classes => classes.AssignableToAny(
                    typeof(IGmailClientFactory),
                    typeof(ICatalogosService),
                    typeof(IEmailService),
                    typeof(IUsuarioEmailRepository),
                    typeof(IJwtFactory),
                    typeof(IJwtService),
                    typeof(IHashStrategy),
                    typeof(IHashService),
                    typeof(IRandomStringGeneratorService),
                    typeof(IThesaurusService),
                    typeof(IUsuarioRepository),
                    typeof(IImportador),
                    typeof(IMigrador),
                    typeof(IOnaMigrate),
                    typeof(IAuthenticateService),
                    typeof(IRecoverUserService),
                    typeof(IConectionStringBuilderService),
                    typeof(IDynamicRepository),
                    typeof(IReporteRepository),
                    typeof(IpaActualizarFiltroRepository),
                    typeof(IEventTrackingRepository),
                    typeof(IThesaurusRepository)
                ))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );

            // También puedes registrar todos los servicios por convención:
            services.Scan(scan => scan
                .FromAssembliesOf(typeof(EmailService), typeof(ONARepository)) // uno por cada proyecto
                .AddClasses()
                .AsMatchingInterface()
                .WithScopedLifetime()
            );

            // HttpContextAccessor como Singleton
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // HttpClient para servicios externos
            services.AddHttpClient<IIpCoordinatesService, IpCoordinatesService>();

        }

        /// <summary>
        /// Configura la autenticación basada en JWT para la aplicación.
        /// </summary>
        /// <param name="services">El contenedor de servicios de la aplicación.</param>
        /// <param name="configuration">La configuración de la aplicación.</param>
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Obtiene la clave secreta de la configuración.
            var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("ApiSettings:Secreta") ?? "");

            // Configura la autenticación JWT.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        /// <summary>
        /// Configura Swagger para la documentación interactiva de la API.
        /// </summary>
        /// <param name="services">El contenedor de servicios de la aplicación.</param>
        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                // Configura la definición de seguridad para usar JWT en Swagger.
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
                    "Ingresa la palabra 'Bearer' seguida de un [espacio] y después tu token en el campo de abajo \r\n\r\n" +
                    "Ejemplo: \"Bearer tkdknkdllskd\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });

                // Configura los requisitos de seguridad para Swagger.
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
          {
            new OpenApiSecurityScheme {
              Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header
            },
            new List<string>()
          }
        });
            });
        }
    }
}
