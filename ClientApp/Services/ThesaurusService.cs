using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using ClientApp.Helpers;
using ClientApp.Models;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClientApp.Services
{
    public class ThesaurusService : IThesaurusService
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlBaseApi;

        public ThesaurusService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _urlBaseApi = $"{Inicializar.UrlBaseApi}api/thesaurus/";
        }
        public async Task<ThesaurusDto> GetThesaurusAsync(string endpoint)
        {
            var response = await _httpClient.GetAsync($"{_urlBaseApi}{endpoint}");
            response.EnsureSuccessStatusCode();
            var respuesta = await response.Content.ReadFromJsonAsync<RespuestasAPI<ThesaurusDto>>();

            if (respuesta != null && respuesta.IsSuccess && respuesta.Result != null)
            {
                return respuesta.Result;
            }

            return default;
        }

        public async Task<RespuestaRegistro> UpdateExpansionAsync(string endpoint, List<ExpansionDto> expansions)
        {
            var content = JsonConvert.SerializeObject(expansions);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            response = await _httpClient.PostAsync($"{_urlBaseApi}{endpoint}", bodyContent);

            if (response.IsSuccessStatusCode)
            {
                return new RespuestaRegistro { registroCorrecto = true };
            }
            else
            {
                return new RespuestaRegistro { registroCorrecto = false }; ;
            }
        }

        public async Task<string> EjecutarBatAsync(string endpoint)
        {
            var response = await _httpClient.GetAsync($"{_urlBaseApi}{endpoint}");
            response.EnsureSuccessStatusCode();
            var respuesta = await response.Content.ReadFromJsonAsync<RespuestasAPI<string>>();

            if (respuesta != null && respuesta.IsSuccess && respuesta.Result != null)
            {
                return respuesta.Result;
            }

            return default;
        }
    }
}