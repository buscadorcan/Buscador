using WebApp.Models;
using WebApp.Service.IService;

namespace WebApp.WorkerService
{
  public class BackgroundExcelService : BackgroundService
  {
    private string? _configLogPath;
    private readonly IConfiguration? _config;
    readonly ILogger<BackgroundExcelService> _logger;
    private readonly IServiceProvider _services;

    public BackgroundExcelService(ILogger<BackgroundExcelService> logger, IConfiguration config, IServiceProvider provider)
    {
      _logger = logger;
      _logger.LogInformation($"\n\n ♨️ Start Background Excel Service: {DateTime.Now}");

      _config = config;
      _configLogPath = _config?.GetConnectionString("LogPath") == null ? "": _config?.GetConnectionString("LogPath");
      _services = provider;
    }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                //using (var scope = _services.CreateScope())
                //{
                //    var excelService = scope.ServiceProvider.GetRequiredService<IExcelService>();
                //    excelService.ImportarExcel(@".\\Files\\cargaDataBusccadorAndino.xlsx", null, 0);
                //    // var service = scope.ServiceProvider.GetRequiredService<IImportador>();
                //    // service.Importar(Array.Empty<string>());
                //}
                //await Task.Delay(TimeSpan.FromMinutes(10000), stoppingToken);
            }
        }
    }
}