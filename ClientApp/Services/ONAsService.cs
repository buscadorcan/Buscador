using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.Win32;
using Newtonsoft.Json;
using SharedApp.Dtos;
using SharedApp.Response;
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
        public async Task<List<OnaDto>> GetONAsAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<OnaDto>>>()).Result;
        }
        public async Task<List<OnaDto>> GetListByONAsAsync(int idOna)
        {
            var response = await _httpClient.GetAsync($"{url}/Lista/{idOna}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<OnaDto>>>()).Result;
        }
        public async Task<OnaDto> GetONAsAsync(int IdONA)
        {
            var response = await _httpClient.GetAsync($"{url}/{IdONA}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<OnaDto>>()).Result;
        }

        public async Task<List<VwPaisDto>> GetPaisesAsync()
        {
            var response = await _httpClient.GetAsync($"{url}/paises");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<VwPaisDto>>>()).Result;
        }

        public async Task<RespuestaRegistro> RegistrarONAsActualizar(OnaDto ONAParaRegistro)
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

        public async Task<bool> DeleteONAAsync(int IdONA)
        {
            var response = await _httpClient.DeleteAsync($"{url}/{IdONA}");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<RespuestasAPI<bool>>();
            return apiResponse?.IsSuccess ?? false;
        }
    }
}
