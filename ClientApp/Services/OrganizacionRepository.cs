using System.Net.Http.Json;
using ClientApp.Helpers;
using ClientApp.Services.IService;

namespace ClientApp.Services {
    public class OrganizacionRepository : IOrganizacionRepository
    {
        private readonly HttpClient _httpClient;

        public OrganizacionRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Organizacion>> BuscarPalabraAsync(string field, string value)
        {
            var response = await _httpClient.GetAsync($"{Inicializar.UrlBaseApi}api/buscador/buscar_palabras?field={field}&value={value}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<Organizacion>>();
        }
    }
}