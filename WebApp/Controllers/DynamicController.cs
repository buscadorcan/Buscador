using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace WebApp.Controllers
{
    [Route("api/vistas")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class DynamicController(IDynamicRepository vhRepo) : BaseController
    {
        private readonly IDynamicRepository _vhRepo = vhRepo;
        [HttpGet("getProperties")]
        public IActionResult GetProperties(int idSystem, string viewName)
        {
            try
            {
                return Ok(new RespuestasAPI<List<PropiedadesTablaDto>> {
                    Result = _vhRepo.GetProperties(idSystem, viewName)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetProperties));
            }
        }
    }
}