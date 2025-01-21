using System.Net;
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

        public async Task<ONAConexionDto> GetConexionAsync(int idConexion)
        {
            var response = await _httpClient.GetAsync($"{url}/{idConexion}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<ONAConexionDto>>()).Result;
        }
        public async Task<ONAConexionDto> GetOnaConexionByOnaAsync(int idOna)
        {
            var response = await _httpClient.GetAsync($"{url}/onaconexion/{idOna}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<ONAConexionDto>>()).Result;
        }
        public async Task<List<ONAConexionDto>> GetConexionsAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<ONAConexionDto>>>()).Result;
        }

        public async Task<RespuestaRegistro> RegistrarOActualizar(ONAConexionDto registro)
        {
            var verificarConexion = await _httpClient.GetAsync($"{url}/{registro.IdONA}");
            var content = JsonConvert.SerializeObject(registro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            if (verificarConexion.IsSuccessStatusCode) // Código 200 OK o similar
            {
                var UrlNew = $"{url}/{registro.IdONA}";
                // Actualizar registro existente
                response = await _httpClient.PutAsync($"{url}/{registro.IdONA}", bodyContent);
            }
            else if (verificarConexion.StatusCode == HttpStatusCode.NotFound) // Código 404
            {
                // Crear nuevo registro
                response = await _httpClient.PostAsync(url, bodyContent);
            }
            else
            {
                // Manejar otros errores (500, etc.)
                throw new Exception($"Error al verificar conexión: {verificarConexion.StatusCode}");
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

        public async Task<HttpResponseMessage> ImportarExcel(MultipartFormDataContent content)
        {
            return await _httpClient.PostAsync($"{url}/upload", content);
        }

        public async Task<RespuestaRegistro> testConexion(int idConexion)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{url}/test/{idConexion}");
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

        public async Task<RespuestaRegistro> migrarConexion(int idConexion)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{url}/migrar/{idConexion}");
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

        public async Task<RespuestaRegistro> DeleteConexionsAsync(int idConexion)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{url}/eliminar/{idConexion}");
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
    }
}