using System.Net.Http.Json;
using Infractruture.Interfaces;
using SharedApp.Dtos;
using SharedApp.Helpers;
using SharedApp.Response;


namespace Infractruture.Services
{
    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlBaseApi;


        
        public EmailService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _urlBaseApi = $"{Inicializar.UrlBaseApi}api/Email/";
        }

        ///<summary>
        ///GetThesaurusAsync: Obtiene el archivo thesauros en formato json
        ///</summary>
        public async Task<bool> Enviar(EmailDto email ,string endpoint)
        {
            var response = await _httpClient.GetAsync($"{_urlBaseApi}{endpoint}");
            response.EnsureSuccessStatusCode();

            var respuesta = await response.Content.ReadFromJsonAsync<RespuestasAPI<bool>>();

            if (respuesta != null && respuesta.IsSuccess && respuesta.Result != null)
            {
                return respuesta.Result;
            }

            return false;
        }
    }
}