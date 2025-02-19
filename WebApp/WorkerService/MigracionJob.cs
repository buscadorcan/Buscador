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

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {}
    }
}