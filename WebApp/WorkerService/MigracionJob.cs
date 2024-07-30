using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service;
using WebApp.Service.IService;

namespace WebApp.WorkerService
{
  public class MigracionJob : BackgroundService
  {
    private string? _configLogPath;
    private readonly IConfiguration? _config;
    readonly ILogger<MigracionJob> _logger;
    private readonly IServiceProvider _services;

    public MigracionJob(ILogger<MigracionJob> logger, IConfiguration config, IServiceProvider provider)
    {
      _logger = logger;
      _logger.LogInformation($"\n\n ♨️ Start Background Migrations Job: {DateTime.Now}");

      _config = config;
      _configLogPath = _config?.GetConnectionString("LogPath") == null ? "": _config?.GetConnectionString("LogPath");
      _services = provider;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      while(!stoppingToken.IsCancellationRequested)
      {
        try
        {
          using (var scope = _services.CreateScope())
          { 
            var service = scope.ServiceProvider.GetRequiredService<IMigrador>();
            var conexionRepository = scope.ServiceProvider.GetRequiredService<IConexionRepository>();
            List<Conexion> conexiones = conexionRepository.FindAll();
            foreach (var conexion in conexiones)
            {
              Console.WriteLine($"Migrando conexión: {conexion.IdSistema}");
              // [TODO]: Validar si tiempo configurado en conexion ya ha pasado
              if (service.Migrar(conexion))
              {
                _logger.LogInformation($"Migración exitosa de la conexión: {conexion.IdSistema}");
                // conexion.Migrar = "N";
                conexion.FechaConexion = DateTime.Now;
                conexionRepository.Update(conexion);
                Environment.Exit(0);
              }
              else
              {
                _logger.LogError($"Error en la migración de la conexión: {conexion.IdSistema}");
              }
              
            }
          }
          // [TODO]: Esperar tiempo predeterminado de ejecución mínima 
          // Se recomienda 1 día, de no ser podible no bajar a menos de 1 hora
          await Task.Delay(TimeSpan.FromMinutes(10000), stoppingToken);
        }
        catch (Exception ex)
        {
          _logger.LogError($"Error en la migración: {ex.Message}");
        }
      }
    }
  }
}