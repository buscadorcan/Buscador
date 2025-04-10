using System;
using System.Net.Http.Json;
using System.Text.Json;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var url = _urlBaseApi;
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetFiltroDetalleAsync<T>(string endpoint, string CodigoHomologacion)
        {
            return await GetApiResponseAsync<T>($"{endpoint}/{CodigoHomologacion}");
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

        public async Task<List<VwFiltroDto>> GetFiltrosAsync()
        {
            var url = _urlBaseApi + "filters/schema";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<VwFiltroDto>>>()).Result;
        }

        public async Task<Dictionary<string, List<vw_FiltrosAnidadosDto>>> GetFiltrosAnidadosAsync(List<FiltrosBusquedaSeleccionDto> filtros)
        {
            var url = _urlBaseApi + "filters/anidados";
            var response = await _httpClient.PostAsJsonAsync(url, filtros);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Dictionary<string, List<vw_FiltrosAnidadosDto>>>()
                   ?? new Dictionary<string, List<vw_FiltrosAnidadosDto>>();
        }


        public async Task<List<vwPanelONADto>> GetPanelOnaAsync()
        {
            var url = _urlBaseApi + "panel";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<vwPanelONADto>>>()).Result;
        }

        public async Task<List<vwONADto>> GetvwOnaAsync()
        {
            var url = _urlBaseApi + "vwona";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<vwONADto>>>()).Result;
        }

        public async Task<List<vwEsquemaOrganizaDto>> GetvwEsquemaOrganizaAsync()
        {
            var url = _urlBaseApi + "EsquemaOrganiza";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<vwEsquemaOrganizaDto>>>()).Result;
        }

    }
}