using System.Net.Http.Json;
using Infractruture.Interfaces;
using SharedApp.Dtos;
using SharedApp.Helpers;
using SharedApp.Response;

namespace Infractruture.Services {
    public class MigracionExcelService : IMigracionExcelService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/migracionexcel";

        public MigracionExcelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
        }

        public async Task<List<MigracionExcelDto>> GetMigracionExcelsAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<MigracionExcelDto>>>()).Result;
        }

        public async Task<HttpResponseMessage> ImportarExcel(MultipartFormDataContent content, int idOna)
        {
            return await _httpClient.PostAsync($"{url}/upload?idOna={idOna}", content);
        }
    }
}