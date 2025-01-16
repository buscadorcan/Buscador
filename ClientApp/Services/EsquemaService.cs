using ClientApp.Helpers;
using ClientApp.Models;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using System.Net.Http.Json;
using System.Text;
using ClientApp.Services.IService;

namespace ClientApp.Services
{
    public class EsquemaService : IEsquemaService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/esquema";

        public EsquemaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<EsquemaDto>> GetListEsquemasAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<EsquemaDto>>>()).Result;
        }

        public async Task<EsquemaDto> GetEsquemaAsync(int idEsquema)
        {
            var response = await _httpClient.GetAsync($"{url}/{idEsquema}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<EsquemaDto>>()).Result;
        }

        public async Task<RespuestaRegistro> RegistrarEsquemaActualizar(EsquemaDto esquemaRegistro)
        {
            var content = JsonConvert.SerializeObject(esquemaRegistro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (esquemaRegistro.IdEsquema > 0)
            {
                response = await _httpClient.PutAsync($"{url}/{esquemaRegistro.IdEsquema}", bodyContent);
            }
            else
            {
                response = await _httpClient.PostAsync($"{url}", bodyContent);
            }

            var contentTemp = await response.Content.ReadAsStringAsync();
            var resultado = JsonConvert.DeserializeObject<RespuestaRegistro>(contentTemp);

            if (response.IsSuccessStatusCode)
            {
                return new RespuestaRegistro { registroCorrecto = true };
            }
            else
            {
                return resultado;
            }
        }

        public async Task<bool> DeleteEsquemaAsync(int IdEsquema)
        {
            var response = await _httpClient.DeleteAsync($"{url}/{IdEsquema}");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<RespuestasAPI<bool>>();
            return apiResponse?.IsSuccess ?? false;
        }
        public async Task<List<EsquemaVistaOnaDto>> GetEsquemaByOnaAsync(int idOna)
        {
            var response = await _httpClient.GetAsync($"{url}/esquemas/{idOna}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<EsquemaVistaOnaDto>>>()).Result;
        }
    }
}
