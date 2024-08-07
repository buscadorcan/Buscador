using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace WebApp.Controllers
{
    [Route("api/vistas")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class DynamicController : BaseController
    {
        private readonly IDynamicRepository _vhRepo;

        public DynamicController(IDynamicRepository vhRepo)
        {
            _vhRepo = vhRepo;
        }

        [HttpGet("columns/{codigoHomologacion}/{viewName}", Name = "getProperties")]
        public IActionResult GetProperties(string codigoHomologacion, string viewName)
        {
            try
            {
                var result = _vhRepo.GetProperties(codigoHomologacion, viewName);
                return Ok(new RespuestasAPI<List<PropiedadesTablaDto>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetProperties));
            }
        }
        [HttpGet("{codigoHomologacion}", Name = "getViewNames")]
        public IActionResult GetViewNames(string codigoHomologacion)
        {
            try
            {
                var result = _vhRepo.GetViewNames(codigoHomologacion);
                return Ok(new RespuestasAPI<List<string>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetViewNames));
            }
        }
    }
}