using System.Net.Http.Json;
using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services
{
    public class BusquedaService(HttpClient httpClient) : IBusquedaService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<BuscadorDto> PsBuscarPalabraAsync(string paramJSON, int PageNumber, int RowsPerPage)
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/search/phrase?paramJSON={paramJSON}&PageNumber={PageNumber}&RowsPerPage={RowsPerPage}");
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

        public async Task<List<FnPredictWordsDto>> FnPredictWords(string word)
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/predictWords?word={word}");
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<FnPredictWordsDto>>>()).Result;
        }
    }
}