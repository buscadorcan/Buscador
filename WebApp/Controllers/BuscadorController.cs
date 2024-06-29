using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;

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
                return Ok(new RespuestasAPI {
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
                return Ok(new RespuestasAPI {
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
                return Ok(new RespuestasAPI {
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
                return Ok(new RespuestasAPI {
                    Result = _vhRepo.FnHomologacionEsquemaDato(idHomologacionEsquema, idDataLakeOrganizacion)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FnHomologacionEsquemaTodo));
            }
        }
    }
}