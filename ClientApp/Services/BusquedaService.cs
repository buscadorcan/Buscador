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

        public async Task<ResultDataHomologacionEsquema> PsBuscarPalabraAsync(string paramJSON, int PageNumber, int RowsPerPage)
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/buscarPalabra?paramJSON={paramJSON}&PageNumber={PageNumber}&RowsPerPage={RowsPerPage}");
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<ResultDataHomologacionEsquema>>()).Result;
        }
        public async Task<List<HomologacionEsquemaDto>> FnHomologacionEsquemaTodoAsync()
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/homologacionEsquemaTodo");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<HomologacionEsquemaDto>>>()).Result;
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
        public async Task<List<DataHomologacionEsquema>> FnHomologacionEsquemaDatoAsync(int idHomologacionEsquema, int idDataLakeOrganizacion)
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/homologacionEsquemaDato/{idHomologacionEsquema}/{idDataLakeOrganizacion}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<DataHomologacionEsquema>>>()).Result;
        }
        public async Task<List<FnPredictWordsDto>> FnPredictWords(string word)
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/predictWords?word={word}");
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<FnPredictWordsDto>>>()).Result;
        }
    }
}