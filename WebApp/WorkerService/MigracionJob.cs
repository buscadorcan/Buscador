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
            _configLogPath = _config?.GetConnectionString("LogPath") == null ? "" : _config?.GetConnectionString("LogPath");
            _services = provider;

        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IMigrador>();
                        var conexionRepository = scope.ServiceProvider.GetRequiredService<IONAConexionRepository>();
                        List<ONAConexion> conexiones = conexionRepository.FindAll().Where(x => x.OrigenDatos != "EXCEL").ToList();
                        foreach (var conexion in conexiones)
                        {
                            Console.WriteLine($"Migrando conexión: {conexion.IdONA}");
                            //var respuesta = await service.MigrarAsync(conexion);
                            //if (respuesta)
                            //{
                            //    conexion.Migrar = "N";
                            //    conexionRepository.Update(conexion);
                            //}
                        }
                    }
                    // Calculate the time until midnight
                    DateTime now = DateTime.Now;
                    DateTime midnight = now.Date.AddDays(1);
                    TimeSpan timeUntilMidnight = midnight - now;

                    // Delay the execution until midnight
                    await Task.Delay(timeUntilMidnight, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error en la migración: {ex.Message}");
                }
            }
        }
    }
}