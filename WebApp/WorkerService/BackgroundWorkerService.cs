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
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    var delay = new TimeSpan();
            //    var currentTime = DateTime.Now;
            //    var startProcessTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, _startGetDataHora, _startGetDataMin, 0);

            //    switch (pais)
            //    {                           // Calcular el tiempo de inicio proceso
            //        case Country.Ninguno:
            //            delay = startProcessTime > currentTime
            //                                        ? startProcessTime - currentTime
            //                                        : startProcessTime.AddDays(1) - currentTime;
            //            pais = Country.Colombia;
            //            msgNextWorker(delay);
            //            await Task.Delay(delay, stoppingToken);
            //            break;
            //        case Country.Colombia:
            //            delay += await GetDataProcessCountry();
            //            pais = Country.Ecuador;
            //            msgNextWorker(delay);
            //            await Task.Delay(delay, stoppingToken);
            //            break;
            //        case Country.Ecuador:
            //            delay += await GetDataProcessCountry();
            //            pais = Country.Peru;
            //            msgNextWorker(delay);
            //            await Task.Delay(delay, stoppingToken);
            //            break;
            //        case Country.Peru:
            //            delay += await GetDataProcessCountry();
            //            pais = Country.Bolivia;
            //            msgNextWorker(delay);
            //            await Task.Delay(delay, stoppingToken);
            //            break;
            //        case Country.Bolivia:
            //            delay += await GetDataProcessCountry();
            //            pais = Country.Ninguno;
            //            break;
            //    }
            //}
        }

        private void msgNextWorker(TimeSpan delay)
        {
            string nextTime = DateTime.Now.Add(delay).ToString("dddd, dd-mm-yy hh:mm:ss tt");
            _logger.LogInformation($"\n\n ⚡ {pais.ToString().ToUpper()} : Worker running at {nextTime}");
        }

        private async Task<TimeSpan> GetDataProcessCountry()
        {
            var contenido = $" 🧬 {pais} >> GetDataProcess[ Start: {DateTime.Now:HH:mm:ss} ]";
            _logger.LogInformation($"\n\n {contenido}");
            countProceso++;
            await File.WriteAllTextAsync($"{_configLogPath}{countProceso}GetDataProcess{pais}.sqlite", ""); //, contenido
            await Task.Delay(1000 * countProceso);

            return DateTime.Now.AddMinutes(_delayProcessMin) - DateTime.Now;
        }

    }
}