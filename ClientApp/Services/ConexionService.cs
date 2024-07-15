using System.Net.Http.Json;
using System.Text;
using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services {
    public class ConexionService : IConexionService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/conexion";

        public ConexionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RespuestaRegistro> EliminarConexion(int idConexion)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{url}/{idConexion}");
            if (response.IsSuccessStatusCode)
            {
                return new RespuestaRegistro { registroCorrecto = true };
            }
            else
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RespuestaRegistro>(contentTemp);
            }
        }

        public async Task<ConexionDto> GetConexionAsync(int idConexion)
        {
            var response = await _httpClient.GetAsync($"{url}/{idConexion}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<ConexionDto>>()).Result;
        }

        public async Task<List<ConexionDto>> GetConexionsAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<ConexionDto>>>()).Result;
        }

        public async Task<RespuestaRegistro> RegistrarOActualizar(ConexionDto registro)
        {
            var content = JsonConvert.SerializeObject(registro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (registro.IdConexion > 0)
            {
                response = await _httpClient.PutAsync($"{url}/{registro.IdConexion}", bodyContent);
            }
            else
            {
                response = await _httpClient.PostAsync(url, bodyContent);
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