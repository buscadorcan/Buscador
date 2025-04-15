using Core.Interfaces;
using DataAccess.Interfaces;
using SharedApp.Dtos;
using System.Text.Json;


namespace Core.Service
{
    public class OnaMigrateService : IOnaMigrate
    {
        private readonly HttpClient _httpClient;
        private readonly IOnaMigrateRepository _ionaMigrateRepository;
        private string UrlBase = "https://onacweb.onac.org.co/onacweb";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="IonaMigrateRepository"></param>
        public OnaMigrateService(HttpClient httpClient, IOnaMigrateRepository IonaMigrateRepository)
        {
            this._httpClient = httpClient;
            _ionaMigrateRepository = IonaMigrateRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="idOna"></param>
        /// <param name="idEsquema"></param>
        /// <returns></returns>
        public async Task<List<OnaMigrateDto>> postOnaMigrate(string view, int idOna, int idEsquema)
        {
            try
            {
                var url = $"{UrlBase}/services/CSONAC.OEC.svc/{view}?ConDatos=true&NroRegistros=10";
                var response = await _httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();

                using var contentStream = await response.Content.ReadAsStreamAsync();
                using var jsonDoc = await JsonDocument.ParseAsync(contentStream);

                if (!jsonDoc.RootElement.TryGetProperty("Datos", out var datosElement))
                {
                    Console.WriteLine("No se encontró la propiedad 'Datos' en la respuesta JSON.");
                    return null;
                }

                var result = _ionaMigrateRepository.postOnaMigrate(idOna, idEsquema, datosElement.GetRawText());

                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error en la solicitud HTTP: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error al parsear el JSON: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado en PostOnaMigrateAsync: {ex.Message}");
                return null;
            }
        }

    }
}
