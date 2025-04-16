using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using SharedApp.Helpers;
using System.Net.Http.Headers;

namespace Infractruture.Services
{
    public class UtilitiesService : IUtilitiesService
    {
        private readonly HttpClient _httpClient;
        private string url = $"{Inicializar.UrlBaseApi}api/Utilities";

        public UtilitiesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> UploadIconAsync(IBrowserFile file, int idONA)
        {
            try
            {
                // Crear el contenido para la solicitud
                var content = new MultipartFormDataContent();

                // Agregar el archivo al contenido del formulario
                var streamContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024)); // Máx. 2MB
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(streamContent, "file", file.Name);

                // Agregar el idONA como otro campo en el formulario
                content.Add(new StringContent(idONA.ToString()), "idONA");

                // Enviar la solicitud al endpoint
                var response = await _httpClient.PostAsync($"{url}/UploadIcon", content);

                // Verificar si la respuesta es exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Obtener la ruta del archivo desde la respuesta
                    var result = await response.Content.ReadAsStringAsync();
                    return result; // Puedes deserializar el JSON si es necesario
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error al cargar el archivo: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al subir el archivo: {ex.Message}");
            }
        }




    }
}
