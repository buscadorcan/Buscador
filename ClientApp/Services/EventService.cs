using ClientApp.Helpers;
using ClientApp.Services.IService;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ClientApp.Services
{
    public class EventService : IEventService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/eventtracking";

        public EventService(HttpClient httpClient)
        {
            this._httpClient = httpClient;
        }

        async Task<List<VwEventUserAllDto>> IEventService.GetListEventUserAllAsync()
        {
            var response = await _httpClient.GetAsync($"{url}/EventUserAll");
            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<VwEventUserAllDto>>>()).Result;
        }

        async Task<List<EventUserDto>> IEventService.GetEventAsync(string report, DateOnly fini, DateOnly ffin)
        {
            var urlWithParams = $"{url}/Even?report={report}&fini={fini:yyyy-MM-dd}&ffin={ffin:yyyy-MM-dd}";

            var response = await _httpClient.GetAsync(urlWithParams);
            response.EnsureSuccessStatusCode();

            return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<EventUserDto>>>()).Result;
        }

         async Task<bool> IEventService.DeleteEventAllAsync(string report, DateOnly fini, DateOnly ffin)
        {
            var urlWithParams = $"{url}/DeleteEven/{fini:yyyy-MM-dd}/{ffin:yyyy-MM-dd}";

            var response = await _httpClient.DeleteAsync(urlWithParams); // Se usa DELETE en lugar de GE
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<RespuestasAPI<bool>>();

            return result?.Result ?? false; // Evita posibles errores de null
        }


        public async Task<bool> DeleteEventByIdAsync(string report, int codigoEvento)
        {  
            var urlWithParams = $"{url}/DeleteEvenById/{codigoEvento}";

            var response = await _httpClient.DeleteAsync(urlWithParams);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<RespuestasAPI<bool>>();

            return result?.Result ?? false; // Evita posibles errores de null
        }

        public async Task<List<VwEventTrackingSessionDto>> GetEventSessionAsync()
        {
            var response = await _httpClient.GetAsync($"{url}/EventSession");
            response.EnsureSuccessStatusCode();

            var sessions = (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<VwEventTrackingSessionDto>>>()).Result;

            foreach (var session in sessions)
            {
                if (session.IpDirec != null)
                {
                    var coordinates = await GetCoordinatesByIPAsync(session.IpDirec);
                    session.Latitud = coordinates?.lat;
                    session.Longitud = coordinates?.lon;
                }
            }

            return sessions;

        }
        private async Task<CoordinatesDto?> GetCoordinatesByIPAsync(string ip)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://ip-api.com/json/{ip}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CoordinatesDto>();
                }
                return null;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("La solicitud se canceló (timeout): " + ex.Message);
                return null;
            }
            catch (Exception ex){
                Console.WriteLine("Error en la consulta de IP: " + ex.Message);
                return null;
            }
        }
    }
}
