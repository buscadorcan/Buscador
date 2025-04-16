using System.Net.Http.Json;
using System.Text;
using Infractruture.Interfaces;
using Infractruture.Models;
using Newtonsoft.Json;
using SharedApp.Dtos;
using SharedApp.Helpers;
using SharedApp.Response;

namespace Infractruture.Services {
    public class HomologacionEsquemaService : IHomologacionEsquemaService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/esquema";

        public HomologacionEsquemaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RespuestaRegistro> EliminarHomologacionEsquema(int idHomologacionEsquema)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{url}/{idHomologacionEsquema}");
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

        public async Task<HomologacionEsquemaDto> GetHomologacionEsquemaAsync(int idHomologacionEsquema)
        {
            var response = await _httpClient.GetAsync($"{url}/{idHomologacionEsquema}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<HomologacionEsquemaDto>>()).Result;
        }

        public async Task<List<HomologacionEsquemaDto>> GetHomologacionEsquemasAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<HomologacionEsquemaDto>>>()).Result;
        }

        public async Task<RespuestaRegistro> RegistrarOActualizar(HomologacionEsquemaDto registro)
        {
            var content = JsonConvert.SerializeObject(registro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (registro.IdHomologacionEsquema > 0)
            {
                response = await _httpClient.PutAsync($"{url}/{registro.IdHomologacionEsquema}", bodyContent);
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