using Infractruture.Interfaces;
using SharedApp.Dtos;
using SharedApp.Helpers;
using SharedApp.Response;
using System.Net.Http.Json;

namespace Infractruture.Services
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
            try
            {
                var response = await _httpClient.GetAsync($"{url}/EventUserAll");
                response.EnsureSuccessStatusCode();
                return (await response.Content.ReadFromJsonAsync<RespuestasAPI<List<VwEventUserAllDto>>>()).Result;
            }
            catch (Exception ex) {
                Console.WriteLine("Error en la consulta: " + ex.Message);
                return null;
            }
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
            try
            {
                var response = await _httpClient.GetAsync($"{url}/EventSession").ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error en la solicitud HTTP: {response.StatusCode}");
                    return new List<VwEventTrackingSessionDto>(); // Devolvemos lista vacía en caso de error
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<RespuestasAPI<List<VwEventTrackingSessionDto>>>()
                    .ConfigureAwait(false);

                var sessions = apiResponse?.Result ?? new List<VwEventTrackingSessionDto>();

                // Ejecutar en paralelo para mejorar el rendimiento
                await Parallel.ForEachAsync(sessions, async (session, _) =>
                {
                    if (!string.IsNullOrWhiteSpace(session.IpDirec))
                    {
                        if(!session.IpDirec.Equals("::1"))
                        {
                            var coordinates = await GetCoordinatesByIPAsync(session.IpDirec);
                            if (coordinates != null)
                            {
                                session.Latitud = coordinates.lat;
                                session.Longitud = coordinates.lon;
                            }
                        }
                    }
                }).ConfigureAwait(false);

                return sessions;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                return new List<VwEventTrackingSessionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en GetEventSessionAsync: {ex.Message}");
                return new List<VwEventTrackingSessionDto>();
            }
        }

        public async Task<List<PaginasMasVisitadaDto>> GetEventPagMasVistAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/EventPagMasVisit").ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error en la solicitud HTTP: {response.StatusCode}");
                    return new List<PaginasMasVisitadaDto>(); // Devolvemos lista vacía en caso de error
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<RespuestasAPI<List<PaginasMasVisitadaDto>>>()
                    .ConfigureAwait(false);

                var sessions = apiResponse?.Result ?? new List<PaginasMasVisitadaDto>();

                // Ejecutar en paralelo para mejorar el rendimiento
                await Parallel.ForEachAsync(sessions, async (session, _) =>
                {
                    if (!string.IsNullOrWhiteSpace(session.IpAddress))
                    {
                        if (!session.IpAddress.Equals("::1"))
                        {
                            var coordinates = await GetCoordinatesByIPAsync(session.IpAddress);
                            if (coordinates != null)
                            {
                                session.Latitud = coordinates.lat;
                                session.Longitud = coordinates.lon;
                            }
                        }
                    }
                }).ConfigureAwait(false);

                return sessions;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                return new List<PaginasMasVisitadaDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en GetEventSessionAsync: {ex.Message}");
                return new List<PaginasMasVisitadaDto>();
            }
        }

        public async Task<List<FiltrosMasUsadoDto>> GetEventFiltroMasUsadAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/EventFiltroMasUsado").ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error en la solicitud HTTP: {response.StatusCode}");
                    return new List<FiltrosMasUsadoDto>(); // Devolvemos lista vacía en caso de error
                }

                var apiResponse = await response.Content
                    .ReadFromJsonAsync<RespuestasAPI<List<FiltrosMasUsadoDto>>>()
                    .ConfigureAwait(false);

                var sessions = apiResponse?.Result ?? new List<FiltrosMasUsadoDto>();

                // Ejecutar en paralelo para mejorar el rendimiento
                await Parallel.ForEachAsync(sessions, async (session, _) =>
                {
                    if (!string.IsNullOrWhiteSpace(session.IpAddress))
                    {
                        if (!session.IpAddress.Equals("::1"))
                        {
                            var coordinates = await GetCoordinatesByIPAsync(session.IpAddress);
                            if (coordinates != null)
                            {
                                session.Latitud = coordinates.lat;
                                session.Longitud = coordinates.lon;
                            }
                        }
                    }
                }).ConfigureAwait(false);

                return sessions;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                return new List<FiltrosMasUsadoDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en GetEventSessionAsync: {ex.Message}");
                return new List<FiltrosMasUsadoDto>();
            }
        }

        private async Task<CoordinatesDto?> GetCoordinatesByIPAsync(string ip)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{url}/coordinates/{ip}");

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
