using System.Net.Http.Json;
using System.Text;
using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services {
    public class HomologacionService : IHomologacionService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/homologacion";

        public HomologacionService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RespuestaRegistro> EliminarHomologacion(int idHomologacion)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{url}/{idHomologacion}");
            if (response.IsSuccessStatusCode)
            {
                return new RespuestaRegistro { registroCorrecto = true };
            }
            else
            {
                var contentTemp = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RespuestasAPI<RespuestaRegistro>>(contentTemp).Result;
            }
        }

        public async Task<HomologacionDto> GetHomologacionAsync(int idHomologacion)
        {
            var response = await _httpClient.GetAsync($"{url}/{idHomologacion}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<HomologacionDto>>()).Result;
        }

        public async Task<List<HomologacionDto>> GetHomologacionsAsync(int value)
        {
            var response = await _httpClient.GetAsync($"{url}/findByParent/{value}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<HomologacionDto>>>()).Result;
        }

        public async Task<RespuestaRegistro> RegistrarOActualizar(HomologacionDto registro)
        {
            var content = JsonConvert.SerializeObject(registro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (registro.IdHomologacion > 0)
            {
                response = await _httpClient.PutAsync($"{url}/{registro.IdHomologacion}", bodyContent);
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