using System.Net.Http.Json;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services {
    public class DynamicService : IDynamicService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/vistas";
        public DynamicService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<PropiedadesTablaDto>> GetProperties(int idSystem, string viewName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/columns/{idSystem}/{viewName}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<PropiedadesTablaDto>>>()).Result;
            }
            catch (System.Exception)
            {
                return new List<PropiedadesTablaDto>();
            }
        }
        public async Task<List<string>> GetViewNames(int idSystem)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/{idSystem}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<string>>>()).Result;
            }
            catch (System.Exception)
            {
                return new List<string>();
            }
        }
    }
}