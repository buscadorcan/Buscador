using ClientApp.Helpers;
using ClientApp.Services.IService;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using System;
using System.Net;
using System.Net.Http.Json;

namespace ClientApp.Services
{
    public class ReporteService : IReporteService
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlBaseApi;

        public ReporteService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _urlBaseApi = $"{Inicializar.UrlBaseApi}api/reportevista/";
        }
        //titulo
        public async Task<HomologacionDto> findByVista(string codigoHomologacion)
        {
            var response = await _httpClient.GetAsync($"{_urlBaseApi}findByVista/{codigoHomologacion}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<HomologacionDto>>()).Result;
        }


        //usuario
        public async Task<T?> GetVwAcreditacionOnaAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwAcreditacionEsquemaAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwEstadoEsquemaAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwOecPaisAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwEsquemaPaisAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwOecFechaAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }

        //read
        public async Task<T?> GetVwProfesionalCalificadoAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwProfesionalOnaAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwProfesionalEsquemaAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwProfesionalFechaAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwCalificaUbicacionAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }

        //can
        public async Task<T?> GetVwBusquedaFechaAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwBusquedaFiltroAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwBusquedaUbicacionAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwActualizacionONAAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }

        //ona
        public async Task<T?> GetVwOrganismoRegistradoAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwOrganizacionEsquemaAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
        }
        public async Task<T?> GetVwOrganismoActividadAsync<T>(string endpoint)
        {
            return await GetApiResponseAsync<T>($"{endpoint}");
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
    }
}
