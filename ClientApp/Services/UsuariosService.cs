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
       // private string url2 = $"{Inicializar.UrlBaseApi}api/usuarios";
        private string url3 = $"{Inicializar.UrlBaseApi}api/roles";
        private string url4 = $"{Inicializar.UrlBaseApi}api/onas";

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
        public async Task<bool> DeleteUsuarioAsync(int IdUsuario)
        {
            var response = await _httpClient.DeleteAsync($"{url}/{IdUsuario}");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<RespuestasAPI<bool>>();
            return apiResponse?.IsSuccess ?? false;
        }
        public async Task<UsuarioDto> GetUsuarioAsync(int IdUsuario)
        {
            var response = await _httpClient.GetAsync($"{url}/{IdUsuario}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<UsuarioDto>>()).Result;
        }
        public async Task<List<VwRolDto>> GetRolesAsync()
        {
            var response = await _httpClient.GetAsync($"{url3}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<VwRolDto>>>()).Result;
        }

        public async Task<List<OnaDto>> GetOnaAsync()
        {
            var response = await _httpClient.GetAsync($"{url4}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<OnaDto>>>()).Result;
        }

       
        public async Task<RespuestaRegistro> RegistrarOActualizar(UsuarioDto registro)
        {
            if (registro.Rol == null)
            {
                registro.Rol = "";
            }

            if (registro.RazonSocial == null)
            {
                registro.RazonSocial = "";
            }


            var content = JsonConvert.SerializeObject(registro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = null;

            try
            {
                // Si el IdUsuario es mayor a 0, realizamos un PUT (actualización)
                if (registro.IdUsuario > 0)
                {
                    response = await _httpClient.PutAsync($"{url}/{registro.IdUsuario}", bodyContent);
                }
                else
                {
                    // Si el IdUsuario es 0 o menor, realizamos un POST (registro)
                    response = await _httpClient.PostAsync($"{url}/registro", bodyContent);
                }

                // Lee el contenido de la respuesta
                var contentTemp = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<RespuestaRegistro>(contentTemp);

                // Si la respuesta es exitosa (status code 2xx)
                if (response.IsSuccessStatusCode)
                {
                    return new RespuestaRegistro { registroCorrecto = true };
                }
                else
                {
                    // Si la respuesta no fue exitosa, retornamos el resultado de la deserialización
                    return resultado ?? new RespuestaRegistro { registroCorrecto = false, mensajeError = "Error desconocido." };
                }
            }
            catch (Exception ex)
            {
                // En caso de una excepción, se maneja el error
                return new RespuestaRegistro
                {
                    registroCorrecto = false,
                    mensajeError = $"Excepción durante la solicitud: {ex.Message}"
                };
            }
        }
    }
}