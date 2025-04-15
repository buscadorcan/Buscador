using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WebApp.WorkerService
{
  //public class BackgroundExcelService : BackgroundService
  //{
  //  private string? _configLogPath;
  //  private readonly IConfiguration? _config;
  //  readonly ILogger<BackgroundExcelService> _logger;
  //  private readonly IServiceProvider _services;

  //  public BackgroundExcelService(ILogger<BackgroundExcelService> logger, IConfiguration config, IServiceProvider provider)
  //  {
  //    _logger = logger;
  //    _logger.LogInformation($"\n\n ♨️ Start Background Excel Service: {DateTime.Now}");

  //    _config = config;
  //    _configLogPath = _config?.GetConnectionString("LogPath") == null ? "": _config?.GetConnectionString("LogPath");
  //    _services = provider;
  //  }

  //      protected async override Task ExecuteAsync(CancellationToken stoppingToken)
  //      {
  //      }
  //  }
}