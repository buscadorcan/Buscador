using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebApp.WorkerService
{
    public class BackgroundWorkerService : BackgroundService
    {
        private enum Country
        {
            Ninguno = 0, Colombia = 1, Ecuador = 2, Peru = 3, Bolivia = 4
        }
        private int countProceso = 0;
        private readonly int _startGetDataHora = 00;
        private readonly int _startGetDataMin = 1;
        private readonly int _delayProcessMin = 1;
        private Country pais = Country.Ninguno;
        private readonly string? _configLogPath;
        private readonly IConfiguration? _config;
        readonly ILogger<BackgroundWorkerService> _logger;
        public BackgroundWorkerService(ILogger<BackgroundWorkerService> logger, IConfiguration config)
        {
            _logger = logger;
            _logger.LogInformation($"\n\n ♨️ Start Background Worker Service: {DateTime.Now}");

            _config = config;
            _configLogPath = _config?.GetConnectionString("LogPath") == null ? "" : _config?.GetConnectionString("LogPath");
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {}

        /// <summary>
        /// Registra en el log el tiempo estimado para la próxima ejecución del proceso.
        /// </summary>
        /// <param name="delay">Tiempo de espera antes de la próxima ejecución.</param>
        private void msgNextWorker(TimeSpan delay)
        {
            string nextTime = DateTime.Now.Add(delay).ToString("dddd, dd-mm-yy hh:mm:ss tt");
            _logger.LogInformation($"\n\n ⚡ {pais.ToString().ToUpper()} : Worker running at {nextTime}");
        }

        /// <summary>
        /// Ejecuta el proceso de obtención de datos para un país específico, 
        /// registrando la ejecución en un archivo y aplicando un retraso antes de finalizar.
        /// </summary>
        /// <returns>Devuelve un <see cref="TimeSpan"/> con el tiempo restante hasta la próxima ejecución.</returns>
        private async Task<TimeSpan> GetDataProcessCountry()
        {
            var contenido = $" 🧬 {pais} >> GetDataProcess[ Start: {DateTime.Now:HH:mm:ss} ]";
            _logger.LogInformation($"\n\n {contenido}");
            countProceso++;
            await File.WriteAllTextAsync($"{_configLogPath}{countProceso}GetDataProcess{pais}.sqlite", ""); 
            await Task.Delay(1000 * countProceso);

            return DateTime.Now.AddMinutes(_delayProcessMin) - DateTime.Now;
        }

    }
}