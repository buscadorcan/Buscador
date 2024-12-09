using System.Net.Http.Json;
using System.Text;
using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services {
    public class MigracionExcelService : IMigracionExcelService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/migracionexcel";

        public MigracionExcelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MigracionExcelDto>> GetMigracionExcelsAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<MigracionExcelDto>>>()).Result;
        }

        public async Task<HttpResponseMessage> ImportarExcel(MultipartFormDataContent content)
        {
            return await _httpClient.PostAsync($"{url}/upload", content);
        }
    }
}