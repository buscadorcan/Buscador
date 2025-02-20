using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using MySqlX.XDevAPI;
using System.Text.Json;

namespace WebApp.Controllers
{
    [Route("api/buscador")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public class BuscadorController(IBuscadorRepository vhRepo) : BaseController
    {
        private readonly IBuscadorRepository _vhRepo = vhRepo;
        
       /* 
       * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
       * WebApp/PsBuscarPalabra: Realiza una búsqueda de palabras clave en la base de datos y devuelve los resultados paginados.
       */
       [HttpGet("search/phrase")]
        public IActionResult PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage)
        {
            try
            {
                return Ok(new RespuestasAPI<BuscadorDto>
                {
                    Result = _vhRepo.PsBuscarPalabra(paramJSON, PageNumber, RowsPerPage)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(PsBuscarPalabra));
            }
        }

         /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnHomologacionEsquemaTodo: Obtiene todos los esquemas homologados en función de una vista y un identificador de ONA.
         */
        [HttpGet("homologacionEsquemaTodo")]
        public IActionResult FnHomologacionEsquemaTodo(string VistaFk, int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<EsquemaDto>>
                {
                    Result = _vhRepo.FnHomologacionEsquemaTodo(VistaFk, idOna)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaTodo));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnHomologacionEsquema: Recupera la información de un esquema homologado específico a partir de su identificador.
         */
        [HttpGet("homologacionEsquema/{idEsquema}")]
        public IActionResult FnHomologacionEsquema(int idEsquema)
        {
            try
            {
                return Ok(new RespuestasAPI<FnEsquemaDto>
                {
                    Result = _vhRepo.FnHomologacionEsquema(idEsquema)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaTodo));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnEsquemaCabecera: Obtiene la cabecera de un esquema a partir de su identificador.
         */
        [HttpGet("fnesquemacabecera/{IdEsquemadata}")]
        public IActionResult FnEsquemaCabecera(int IdEsquemadata)
        {
            try
            {
                return Ok(new RespuestasAPI<fnEsquemaCabeceraDto>
                {
                    Result = _vhRepo.FnEsquemaCabecera(IdEsquemadata)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnEsquemaCabecera));
            }
        }


        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnHomologacionEsquemaDato: Obtiene los datos de homologación de un esquema en función de su identificador, vista y ONA.
         */
        [HttpGet("homologacionEsquemaDato/{idEsquema}/{idOna}")]
        public IActionResult FnHomologacionEsquemaDato(int idEsquema,string VistaFK, int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnHomologacionEsquemaDataDto>>
                {
                    Result = _vhRepo.FnHomologacionEsquemaDato(idEsquema, VistaFK, idOna)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaDato));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnEsquemaDato: Realiza una búsqueda de datos dentro de un esquema específico según el texto ingresado.
         */
        [HttpGet("EsquemaDatoBuscado")]
        public IActionResult FnEsquemaDato(int idEsquemaData, string TextoBuscar)
        {
            try
            {
                var result = _vhRepo.FnEsquemaDatoBuscar(idEsquemaData, TextoBuscar);
                return Ok(new RespuestasAPI<List<FnEsquemaDataBuscadoDto>>
                {
                    Result = result
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnEsquemaDato));
            }
        }


        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FnPredictWords: Predice palabras basadas en el inicio de una palabra ingresada.
         */
        [HttpGet("predictWords")]
        public IActionResult FnPredictWords(string word)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnPredictWordsDto>>
                {
                    Result = _vhRepo.FnPredictWords(word)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnPredictWords));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/ValidateWords: Valida una lista de palabras ingresadas por el usuario.
         */
        [HttpPost("validateWords")]
        public IActionResult ValidateWords([FromBody] List<string> words)
        {
            try
            {
                return Ok(new RespuestasAPI<bool>
                {
                    Result = _vhRepo.ValidateWords(words)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ValidateWords));
            }
        }

        /* 
        * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        * WebApp/AddEventTracking: Registra la accion realizada por algun control.
        */
        [HttpPost("addEventTracking")]
        public IActionResult AddEventTracking([FromBody] EventTrackingDto eventTracking)
        {
            try
            {
                // Obtener la IP del cliente
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                // Obtener datos del país
                var ipInfo = GetIpLocationInfo(ipAddress);

                eventTracking.UbicacionJson = JsonSerializer.Serialize(new
                {
                    IpAddress = ipAddress,
                    Country = ipInfo.Result?.Country,
                    City = ipInfo.Result?.City,
                    Isp = ipInfo.Result?.Isp
                });

                if (eventTracking == null)
                    return BadRequest("El objeto EventTracking no puede ser nulo.");

                _vhRepo.AddEventTracking(eventTracking);

                return Ok(new RespuestasAPI<string>
                {
                    Result = "Evento registrado con éxito."
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AddEventTracking));
            }
        }

        /* 
        * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        * WebApp/GetCoordinates: Obtiene coordenadas geográficas (latitud y longitud) de una dirección dada mediante la API de Google Maps.
        */
        [HttpGet("geocode")]
        public async Task<IActionResult> GetCoordinates([FromQuery] string address)
        {
            try
            {
                var apiKey = "AIzaSyC7NUCEvrqrrQDDDRLK2q0HSqswPxtBVAk"; // Reemplaza con tu API Key
                var url = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(address)}&key={apiKey}";

                using var httpClient = new HttpClient();
                var response = await httpClient.GetStringAsync(url);

                return Ok(response);
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetCoordinates));
            }
        }

        private async Task<IpLocationDto> GetIpLocationInfo(string ipAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1") // Verifica si es localhost
                    return new IpLocationDto { Country = "Local", City = "Local", Isp = "Local" };

                using var httpClient = new HttpClient();
                var url = $"http://ip-api.com/json/{ipAddress}"; // API gratuita de geolocalización
                var response = await httpClient.GetStringAsync(url);

                return JsonSerializer.Deserialize<IpLocationDto>(response);
            }
            catch
            {
                return new IpLocationDto { Country = "Desconocido", City = "Desconocido", Isp = "Desconocido" };
            }
        }

    }
}
