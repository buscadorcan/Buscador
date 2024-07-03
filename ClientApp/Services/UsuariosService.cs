using System.Net.Http.Json;
using System.Text;
using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services {
    public class UsuariosService : IUsuariosService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/usuarios";

        public UsuariosService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<UsuarioDto>> GetUsuariosAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<UsuarioDto>>>()).Result;
        }

        public async Task<UsuarioDto> GetUsuarioAsync(int IdUsuario)
        {
            var response = await _httpClient.GetAsync($"{url}/{IdUsuario}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<UsuarioDto>>()).Result;
        }
        public async Task<RespuestaRegistro> RegistrarOActualizar(UsuarioDto registro)
        {
            var content = JsonConvert.SerializeObject(registro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (registro.IdUsuario > 0)
            {
                response = await _httpClient.PutAsync($"{url}/{registro.IdUsuario}", bodyContent);
            }
            else
            {
                response = await _httpClient.PostAsync($"{url}/registro", bodyContent);
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