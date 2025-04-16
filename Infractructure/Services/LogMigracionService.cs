using System.Net.Http.Json;
using Infractruture.Interfaces;
using SharedApp.Dtos;
using SharedApp.Helpers;
using SharedApp.Response;

namespace Infractruture.Services {
    public class LogMigracionService : ILogMigracionService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/logmigracion";

        public LogMigracionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<LogMigracionDto>> GetLogMigracionesAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<LogMigracionDto>>>()).Result;
        }
    }
}