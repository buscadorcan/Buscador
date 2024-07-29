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
      _logger.LogInformation($"\n\n ♨️ Start Background Excel Service: {DateTime.Now}");

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
            // Agregar obtensión de vistas de base de datos
            foreach (var conexion in conexiones)
            {
              if (service.Migrar(conexion))
              {
                _logger.LogInformation($"Migración exitosa de la conexión: {conexion.IdSistema}");
                conexion.Migrar = "N";
                conexion.FechaConexion = DateTime.Now;
                conexionRepository.Update(conexion);
              }
              else
              {
                _logger.LogError($"Error en la migración de la conexión: {conexion.IdSistema}");
              }
              
            }
          }
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