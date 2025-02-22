using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using System.Net.Http.Json;
using System.Text;

namespace ClientApp.Services
{
    public class MenuService : IMenuService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/menu";

        public MenuService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MenuRolDto>> GetMenusAsync()
        {
            var response = await _httpClient.GetAsync($"{url}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<MenuRolDto>>>()).Result;
        }

        public async Task<MenuRolDto> GetMenuAsync(int idHRol, int idHMenu)
        {
            var response = await _httpClient.GetAsync($"{url}/{idHRol}/{idHMenu}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<MenuRolDto>>()).Result;
        }

        public async Task<RespuestaRegistro> RegistrarMenuActualizar(MenuRolDto menuParaRegistro)
        {
            var content = JsonConvert.SerializeObject(menuParaRegistro);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            response = await _httpClient.PostAsync($"{url}", bodyContent);
            //if (menuParaRegistro.IdHRol > 0 && menuParaRegistro.IdHMenu > 0)
            //{
            //    response = await _httpClient.PutAsync($"{url}/{menuParaRegistro.IdHRol}/{menuParaRegistro.IdHMenu}", bodyContent);
            //}
            //else
            //{
            //    response = await _httpClient.PostAsync($"{url}", bodyContent);
            //}

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

        public async Task<bool> DeleteMenuAsync(int? idHRol, int? idHMenu)
        {
            var response = await _httpClient.DeleteAsync($"{url}/{idHRol}/{idHMenu}");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            var apiResponse = await response.Content.ReadFromJsonAsync<RespuestasAPI<bool>>();
            return apiResponse?.IsSuccess ?? false;
        }
        public async Task<List<MenuPaginaDto>> GetMenusPendingConfigAsync(int? idHomologacionRol)
        {
            var response = await _httpClient.GetAsync($"{url}/{idHomologacionRol}");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<MenuPaginaDto>>>()).Result;
        }
    }
}
