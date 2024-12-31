using System;
using System.Net.Http.Json;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services
{
    public class CatalogosService : ICatalogosService
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlBaseApi;
        public CatalogosService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _urlBaseApi = $"{Inicializar.UrlBaseApi}api/catalogos/";
        }
        public async Task<T?> GetHomologacionAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetHomologacionDetalleAsync<T>(string endpoint, int IdHomologacion)
        {
            return await GetApiResponseAsync<T>($"{endpoint}/{IdHomologacion}");
        }
        private async Task<T?> GetApiResponseAsync<T>(string apiEndpoint)
        {
            var response = await _httpClient.GetAsync($"{_urlBaseApi}{apiEndpoint}");
            response.EnsureSuccessStatusCode();
            var respuesta = await response.Content.ReadFromJsonAsync<RespuestasAPI<T>>();

            if (respuesta != null && respuesta.IsSuccess && respuesta.Result != null)
            {
                return respuesta.Result;
            }

            return default;
        }

        public async Task<List<VwMenuDto>> GetMenusAsync()
        {
            var url = _urlBaseApi + "menu";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<VwMenuDto>>>()).Result;
        }
    }
}