using System.Net.Http.Json;
using Infractruture.Interfaces;
using Newtonsoft.Json;
using SharedApp.Dtos;
using SharedApp.Helpers;
using SharedApp.Response;

namespace Infractruture.Services {
    public class DynamicService : IDynamicService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/vistas";
        public DynamicService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<PropiedadesTablaDto>> GetProperties(string codigoHomologacion, string viewName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/columns/{codigoHomologacion}/{viewName}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<PropiedadesTablaDto>>>()).Result;
            }
            catch (System.Exception)
            {
                return new List<PropiedadesTablaDto>();
            }
        }
        //public async Task<List<EsquemaVistaDto>> GetListaValidacionEsquema(int idOna, int idEsquemaVista)
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync($"{url}/validacion/{idOna}/{idEsquemaVista}");
        //        response.EnsureSuccessStatusCode();
        //        return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<EsquemaVistaDto>>>()).Result;
        //    }
        //    catch (System.Exception ex )
        //    {
        //        return new List<EsquemaVistaDto>();
        //    }
        //}
        public async Task<List<PropiedadesTablaDto>> GetValueColumna(int idONA, string valueColumn, string viewName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/columns/{idONA}/{valueColumn}/{viewName}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<PropiedadesTablaDto>>>()).Result;
            }
            catch (System.Exception)
            {
                return new List<PropiedadesTablaDto>();
            }
        }
        public async Task<List<EsquemaVistaDto>> GetListaValidacionEsquema(int idOna, int idEsquema)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/validacion/{idOna}/{idEsquema}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<EsquemaVistaDto>>>()).Result;
            }
            catch (System.Exception ex)
            {
                return new List<EsquemaVistaDto>();
            }
        }
        public async Task<List<string>> GetViewNames(string codigoHomologacion)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/{codigoHomologacion}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<string>>>()).Result;
            }
            catch (System.Exception ex)
            {
                return new List<string>();
            }
        }

        public async Task<bool> TestConnectionAsync(int idOna)
        {
            try
            {
                // Realizar la solicitud al endpoint de prueba de conexión
                var response = await _httpClient.GetAsync($"{url}/test/{idOna}");
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Respuesta JSON: {jsonString}");
                var result = JsonConvert.DeserializeObject<TestConnectionResponse>(jsonString);
                return result?.IsSuccess ?? false;
            }
            catch (Exception ex)
            {
                // Manejar errores
                Console.WriteLine($"Error en TestConnectionAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> MigrarConexionAsync(int idOna)
        {
            try
            {
                // Realizar la solicitud al endpoint de migración
                var response = await _httpClient.PostAsync($"{url}/migrar/{idOna}", null);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TestConnectionResponse>(jsonString);
                return result?.IsSuccess ?? false;
            }
            catch (Exception ex)
            {
                // Manejar errores y devolver un mensaje de error
                Console.WriteLine($"Error en MigrarConexionAsync: {ex.Message}");
                return false;
            }
        }

        private class TestConnectionResponse
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
        }
    }
}