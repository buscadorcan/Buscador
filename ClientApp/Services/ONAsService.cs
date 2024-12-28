using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.Win32;
using Newtonsoft.Json;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using System.Net.Http.Json;
using System.Text;

namespace ClientApp.Services
{
    public class ONAsService : IONAService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/ona";

        public ONAsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<ONADto>> GetONAsAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<ONADto>>>()).Result;
        }

        public async Task<ONADto> GetONAsAsync(int IdONA)
        {
            var response = await _httpClient.GetAsync($"{url}/{IdONA}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<ONADto>>()).Result;
        }

        public async Task<List<VwPaisDto>> GetPaisesAsync()
        {
            var response = await _httpClient.GetAsync($"{url}/paises");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<VwPaisDto>>>()).Result;
        }

        public async Task<RespuestaRegistro> RegistrarONAsActualizar(ONADto ONAParaRegistro)
        {
            var content = JsonConvert.SerializeObject(ONAParaRegistro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (ONAParaRegistro.IdONA > 0)
            {
                response = await _httpClient.PutAsync($"{url}/{ONAParaRegistro.IdONA}", bodyContent);
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
    }
}
