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
        [HttpGet("buscarPalabra")]
        public IActionResult PsBuscarPalabra(string paramJSON, int PageNumber, int RowsPerPage)
        {
            try
            {
                return Ok(new RespuestasAPI<BuscadorDto>{
                    Result = _vhRepo.PsBuscarPalabra(paramJSON, PageNumber, RowsPerPage)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(PsBuscarPalabra));
            }
        }
        
        [HttpGet("homologacionEsquemaTodo")]  
        public IActionResult FnHomologacionEsquemaTodo()
        {
            try
            {
                return Ok(new RespuestasAPI<List<EsquemaDto>>{
                    Result = _vhRepo.FnHomologacionEsquemaTodo()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaTodo));
            }
        }
        
        [HttpGet("homologacionEsquema/{idHomologacionEsquema}")]
        public IActionResult FnHomologacionEsquema(int idHomologacionEsquema)
        {
            try
            {
                return Ok(new RespuestasAPI<FnHomologacionEsquemaDto>{
                    Result = _vhRepo.FnHomologacionEsquema(idHomologacionEsquema)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaTodo));
            }
        }
        
        [HttpGet("homologacionEsquemaDato/{idHomologacionEsquema}/{idDataLakeOrganizacion}")]
        public IActionResult FnHomologacionEsquemaDato(int idHomologacionEsquema, int idDataLakeOrganizacion)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnHomologacionEsquemaDataDto>>{
                    Result = _vhRepo.FnHomologacionEsquemaDato(idHomologacionEsquema, idDataLakeOrganizacion)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaDato));
            }
        }
        [HttpGet("predictWords")]
        public IActionResult FnPredictWords(string word)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnPredictWordsDto>>{
                    Result = _vhRepo.FnPredictWords(word)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnPredictWords));
            }
        }
    }
}