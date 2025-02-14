using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;

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

        [HttpPost("addEventTracking")]
        public IActionResult AddEventTracking([FromBody] EventTrackingDto eventTracking)
        {
            try
            {
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

        [HttpGet("geocode")]
        public async Task<IActionResult> GetCoordinates([FromQuery] string address)
        {
            try
            {
                var apiKey = "AIzaSyBfJMTooTtkGw_hZC3DenLPlEpjq-t_7nY"; // Reemplaza con tu API Key
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

    }
}
