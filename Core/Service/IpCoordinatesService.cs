using System.Net.Http.Json;
using Core.Interfaces;
using SharedApp.Dtos;

namespace Core.Service
{
    /// <summary>
    /// Servicio para obtener coordenadas geográficas basadas en la dirección IP.
    /// </summary>
    public class IpCoordinatesService : IIpCoordinatesService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Constructor del servicio de coordenadas IP.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP inyectado para realizar solicitudes.</param>
        public IpCoordinatesService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Obtiene las coordenadas geográficas de una dirección IP.
        /// </summary>
        /// <param name="ip">Dirección IP a consultar.</param>
        /// <returns>Objeto <see cref="CoordinatesDto"/> con la información de ubicación o <c>null</c> si no se encuentra.</returns>
        public async Task<CoordinatesDto> GetCoordinates(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("La dirección IP no puede estar vacía.", nameof(ip));

            try
            {
                var response = await _httpClient.GetAsync($"http://ip-api.com/json/{ip}").ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    // Regresar null si la API responde con error
                    return null;
                }

                // Leer el JSON con validación de null
                return await response.Content.ReadFromJsonAsync<CoordinatesDto>().ConfigureAwait(false);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en GetCoordinates: {ex.Message}");
                return null;
            }
        }
    }
}
