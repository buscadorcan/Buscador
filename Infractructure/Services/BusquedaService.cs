using System.Net.Http.Json;
using System.Text;
using Infractruture.Interfaces;
using Infractruture.Models;
using Newtonsoft.Json;
using SharedApp.Dtos;
using SharedApp.Helpers;
using SharedApp.Response;

namespace Infractruture.Services
{
    public class BusquedaService(HttpClient httpClient) : IBusquedaService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<BuscadorDto> PsBuscarPalabraAsync(string paramJSON, int PageNumber, int RowsPerPage)
        {
            string encodedJson = Uri.EscapeDataString(paramJSON);
            var Url = Inicializar.UrlBaseApi;
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/search/phrase?paramJSON={encodedJson}&PageNumber={PageNumber}&RowsPerPage={RowsPerPage}");
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<BuscadorDto>>()).Result;
        }
        public async Task<List<HomologacionEsquemaDto>> FnHomologacionEsquemaTodoAsync(string vistaFK, int idOna)
        {
            try
            {
                var url = $"{Inicializar.UrlBaseApi}api/buscador/homologacionEsquemaTodo?VistaFk={Uri.EscapeDataString(vistaFK)}&idOna={idOna}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<RespuestasAPI<List<HomologacionEsquemaDto>>>();
                return result?.Result ?? new List<HomologacionEsquemaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en FnHomologacionEsquemaTodoAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<HomologacionEsquemaDto?> FnHomologacionEsquemaAsync(int idHomologacionEsquema)
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/homologacionEsquema/{idHomologacionEsquema}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<RespuestasAPI<HomologacionEsquemaDto>>();

            if (result != null)
            {
                return result.Result;
            }

            return default;
        }
        public async Task<List<DataHomologacionEsquema>> FnHomologacionEsquemaDatoAsync(int idHomologacionEsquema, string VistaFK, int idOna)
        {
            try
            {
                var url = $"{Inicializar.UrlBaseApi}api/buscador/homologacionEsquemaDato/{idHomologacionEsquema}/{idOna}?VistaFK={Uri.EscapeDataString(VistaFK)}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<RespuestasAPI<List<DataHomologacionEsquema>>>();
                return result?.Result ?? new List<DataHomologacionEsquema>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en FnHomologacionEsquemaDatoAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<List<DataEsquemaDatoBuscar>> FnEsquemaDatoBuscarAsync(int idEsquemaData, string TextoBuscar)
        {
            try
            {
                // Construcción correcta de la URL con los parámetros adecuados
                var url = $"{Inicializar.UrlBaseApi}api/buscador/EsquemaDatoBuscado?idEsquemaData={idEsquemaData}&TextoBuscar={Uri.EscapeDataString(TextoBuscar)}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<RespuestasAPI<List<DataEsquemaDatoBuscar>>>();

                return result?.Result ?? new List<DataEsquemaDatoBuscar>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en FnEsquemaDatoBuscarAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<fnEsquemaCabeceraDto?> FnEsquemaCabeceraAsync(int IdEsquemadata)
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/fnesquemacabecera/{IdEsquemadata}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<RespuestasAPI<fnEsquemaCabeceraDto>>();

            if (result != null)
            {
                return result.Result;
            }

            return default;
        }

        public async Task<List<FnPredictWordsDto>> FnPredictWords(string word)
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/predictWords?word={word}");
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<FnPredictWordsDto>>>()).Result;
        }

        public  async Task<bool> ValidateWords(List<string> words)
        {
            
            var content = JsonConvert.SerializeObject(words);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"{Inicializar.UrlBaseApi}api/buscador/validateWords", bodyContent);

            if (response.IsSuccessStatusCode)
            {
                var respuesta = await response.Content.ReadFromJsonAsync<RespuestasAPI<bool>>();
                return respuesta?.Result ?? false; 
            }
            else
            {
                return false; 
            }

        }

        public async Task<bool> AddEventTrackingAsync(EventTrackingDto eventTracking)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{Inicializar.UrlBaseApi}api/buscador/addEventTracking", eventTracking);

                response.EnsureSuccessStatusCode(); // Lanza una excepción si la respuesta no es exitosa

                var result = await response.Content.ReadFromJsonAsync<RespuestasAPI<string>>();

                return result != null && result.Result == "Evento registrado con éxito.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AddEventTrackingAsync: {ex.Message}");
                return false; // Devuelve false en caso de error
            }
        }

        public async Task<GeocodeResponseDto?> ObtenerCoordenadasAsync(string pais, string ciudad)
        {
            try
            {
                // Llamar al backend en lugar de Google Maps directamente
                var url = $"{Inicializar.UrlBaseApi}api/buscador/geocode?address={Uri.EscapeDataString($"{ciudad}, {pais}")}";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<GeocodeResponseDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerCoordenadasAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> ExportarExcelAsync(List<BuscadorResultadoDataDto> data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{Inicializar.UrlBaseApi}api/buscador/excel", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task<byte[]> ExportarPdfAsync(List<BuscadorResultadoDataDto> data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{Inicializar.UrlBaseApi}api/buscador/pdf", content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }

        Task<List<DataHomologacionEsquema>> IBusquedaService.FnHomologacionEsquemaDatoAsync(int idHomologacionEsquema, string VistaFK, int idOna)
        {
            throw new NotImplementedException();
        }

        Task<List<DataEsquemaDatoBuscar>> IBusquedaService.FnEsquemaDatoBuscarAsync(int idEsquemaData, string TextoBuscar)
        {
            throw new NotImplementedException();
        }
    }
}