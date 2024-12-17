using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
    [HttpGet("columns/{idOna}/{viewName}", Name = "getProperties")]
    public IActionResult GetProperties(int idOna, string viewName)
    {
      try
      {
        var result = _vhRepo.GetProperties(idOna, viewName);
        return Ok(new RespuestasAPI<List<PropiedadesTablaDto>> { Result = result });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(GetProperties));
      }
    }
    [Authorize]
    [HttpGet("{idOna}", Name = "getViewNames")]
    public IActionResult GetViewNames(int idOna)
    {
      try
      {
        var result = _vhRepo.GetViewNames(idOna);
        return Ok(new RespuestasAPI<List<string>> { Result = result });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(GetViewNames));
      }
    }
  }
}
