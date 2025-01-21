using System.Net.Http.Json;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace ClientApp.Services {
    public class DynamicService : IDynamicService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/vistas";
        public DynamicService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<List<PropiedadesTablaDto>> GetProperties(string codigoHomologacion, string viewName)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/columns/{codigoHomologacion}/{viewName}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<PropiedadesTablaDto>>>()).Result;
            }
            catch (System.Exception)
            {
                return new List<PropiedadesTablaDto>();
            }
        }
        //public async Task<List<EsquemaVistaDto>> GetListaValidacionEsquema(int idOna, int idEsquemaVista)
        //{
        //    try
        //    {
        //        var response = await _httpClient.GetAsync($"{url}/validacion/{idOna}/{idEsquemaVista}");
        //        response.EnsureSuccessStatusCode();
        //        return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<EsquemaVistaDto>>>()).Result;
        //    }
        //    catch (System.Exception ex )
        //    {
        //        return new List<EsquemaVistaDto>();
        //    }
        //}
        public async Task<List<EsquemaVistaDto>> GetListaValidacionEsquema(int idOna, int idEsquema)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/validacion/{idOna}/{idEsquema}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<EsquemaVistaDto>>>()).Result;
            }
            catch (System.Exception ex)
            {
                return new List<EsquemaVistaDto>();
            }
        }
        public async Task<List<string>> GetViewNames(string codigoHomologacion)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/{codigoHomologacion}");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<string>>>()).Result;
            }
            catch (System.Exception ex)
            {
                return new List<string>();
            }
        }
    }
}