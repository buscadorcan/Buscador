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
    public static class ServiceExtensions
    {
        public static void ConfigureDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<SqlServerDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Mssql-CanDb")));

            services.AddScoped<ISqlServerDbContextFactory>(provider =>
            {
                var options = provider.GetRequiredService<DbContextOptions<SqlServerDbContext>>();
                return new SqlServerDbContextFactory(options);
            });

            services.AddSingleton<IDbContextFactory, DbContextFactory>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<ISmtpClientFactory, SmtpClientFactory>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtFactory, JwtFactory>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IHashStrategy, Md5HashStrategy>();
            services.AddScoped<IHashService, HashService>();
            services.AddScoped<IPasswordGenerationStrategy, RandomPasswordGenerationStrategy>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IImportador, ImportadorService>();
            services.AddScoped<IMigrador, Migrador>();
            services.AddScoped<IEndpointRepository, EndpointRepository>();
            services.AddScoped<IUsuarioEndpointPermisoRepository, UsuarioEndpointPermisoRepository>();
            services.AddScoped<IBuscadorRepository, BuscadorRepository>();
            services.AddScoped<ICatalogosRepository, CatalogosRepository>();
            services.AddScoped<IHomologacionRepository, HomologacionRepository>();
            services.AddScoped<IHomologacionEsquemaRepository, HomologacionEsquemaRepository>();
            services.AddScoped<ICanDataSetRepository, CanDataSetRepository>();
            services.AddScoped<ICanFullTextRepository, CanFullTextRepository>();
            services.AddScoped<IConexionRepository, ConexionRepository>();
            services.AddScoped<IConectionStringBuilderService, ConectionStringBuilderService>();
            services.AddScoped<IDynamicRepository, DynamicRepository>();
            services.AddScoped<IMigracionExcelRepository, MigracionExcelRepository>();

            // WorkerService
            services.AddHostedService<BackgroundWorkerService>();
            services.AddHostedService<BackgroundExcelService>();
            services.AddHostedService<MigracionJob>();

            // Agregar Automapper
            services.AddAutoMapper(typeof(Mapper));
        }

        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var key = Encoding.ASCII.GetBytes(configuration.GetValue<string>("ApiSettings:Secreta") ?? "");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
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

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                    "Autenticación JWT usando el esquema Bearer. \r\n\r\n " +
                    "Ingresa la palabra 'Bearer' seguida de un [espacio] y después tu token en el campo de abajo \r\n\r\n" +
                    "Ejemplo: \"Bearer tkdknkdllskd\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
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
