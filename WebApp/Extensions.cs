using WebApp.Service;
using WebApp.Repositories;
using WebApp.Service.IService;
using WebApp.Repositories.IRepositories;
using WebApp.WorkerService;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApp.Mappers;

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
            // Registra servicios relacionados con correos electrónicos.
            services.AddScoped<ISmtpClientFactory, SmtpClientFactory>();
            services.AddScoped<IEmailService, EmailService>();

            // Registra servicios relacionados con JWT (autenticación basada en tokens).
            services.AddScoped<IJwtFactory, JwtFactory>();
            services.AddScoped<IJwtService, JwtService>();

            // Registra servicios relacionados con hash y generación de contraseñas.
            services.AddScoped<IHashStrategy, Md5HashStrategy>();
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IPasswordGenerationStrategy, RandomPasswordGenerationStrategy>();
            services.AddScoped<IPasswordService, PasswordService>();

            // Proporciona acceso al contexto HTTP actual.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Registra repositorios para acceso a datos.
            services.AddScoped<IONARepository, ONARepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IImportador, ImportadorService>();
            services.AddScoped<IMigrador, Migrador>();
            services.AddScoped<IUsuarioEndpointRepository, UsuarioEndpointRepository>();
            services.AddScoped<IBuscadorRepository, BuscadorRepository>();
            services.AddScoped<ICatalogosRepository, CatalogosRepository>();
            services.AddScoped<IHomologacionRepository, HomologacionRepository>();
            services.AddScoped<IEsquemaRepository, EsquemaRepository>();
            services.AddScoped<IEsquemaVistaRepository, EsquemaVistaRepository>();
            services.AddScoped<IEsquemaVistaColumnaRepository, EsquemaVistaColumnaRepository>();
            services.AddScoped<IEsquemaDataRepository, EsquemaDataRepository>();
            services.AddScoped<IEsquemaFullTextRepository, EsquemaFullTextRepository>();
            services.AddScoped<IONAConexionRepository, ONAConexionRepository>();
            services.AddScoped<IConectionStringBuilderService, ConectionStringBuilderService>();
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddScoped<IMigracionExcelRepository, MigracionExcelRepository>();
            services.AddScoped<ILogMigracionRepository, LogMigracionRepository>();

            // Registra servicios de trabajo en segundo plano (Worker Services).
            services.AddHostedService<BackgroundWorkerService>();
            services.AddHostedService<BackgroundExcelService>();
            //services.AddHostedService<MigracionJob>();


            // Configura AutoMapper para mapear entre modelos.
            services.AddAutoMapper(typeof(Mapper));
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
