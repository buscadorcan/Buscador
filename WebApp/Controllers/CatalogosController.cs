using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using WebApp.Models;
using SharedApp.Models.Dtos;

namespace WebApp.Controllers
{
    [Route("api/catalogos")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class CatalogosController(
        ICatalogosRepository vhRepo
    ) : BaseController
    {
        private readonly ICatalogosRepository _vhRepo = vhRepo;
        [HttpGet("etiquetas_grilla")]
        public IActionResult ObtenerEtiquetaGrilla()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwGrilla>>{
                    Result = _vhRepo.ObtenerEtiquetaGrilla()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerEtiquetaGrilla));
            }
        }

        [HttpGet("etiquetas_filtro")]
        public IActionResult ObtenerEtiquetaFiltros()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwFiltro>>{
                    Result = _vhRepo.ObtenerEtiquetaFiltros()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerEtiquetaFiltros));
            }
        }
        [HttpGet("dimension")]
        public IActionResult ObtenerDimension()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwDimension>>{
                    Result = _vhRepo.ObtenerDimension()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerDimension));
            }
        }
        [HttpGet("grupo")]
        public IActionResult ObtenerGrupos()
        {
            try
            {
                return Ok(new RespuestasAPI<List<Homologacion>>{
                    Result = _vhRepo.ObtenerGrupos()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerGrupos));
            }
        }

        [HttpGet("filtro_detalles/{idHomologacion:int}", Name = "ObtenerFiltroDetalles")]
        public IActionResult ObtenerFiltroDetalles(int idHomologacion)
        {
            try
            {
                return Ok(new RespuestasAPI<List<FnFiltroDetalleDto>>{
                    Result = _vhRepo.ObtenerFiltroDetalles(idHomologacion)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerFiltroDetalles));
            }
        }
    }
}