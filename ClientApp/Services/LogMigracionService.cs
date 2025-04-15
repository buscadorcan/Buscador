using System.Net.Http.Json;
using System.Text;
using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Dtos;
using SharedApp.Response;

namespace ClientApp.Services {
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