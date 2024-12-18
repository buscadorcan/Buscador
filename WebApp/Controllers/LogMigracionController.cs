using WebApp.Models;
using WebApp.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Models.Dtos;
using SharedApp.Models;

namespace WebApp.Controllers
{
  [Route("api/logmigracion")]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public class LogMigracionController(
    ILogMigracionRepository iRepo,
    IMapper mapper
  ) : BaseController
  {
    private readonly ILogMigracionRepository _iRepo = iRepo;
    private readonly IMapper _mapper = mapper;
    [Authorize]
    [HttpGet]
    public IActionResult FindAll()
    {
      try
      {
        return Ok(new RespuestasAPI<List<LogMigracionDto>> {
          Result = _iRepo.FindAll().Select(item => _mapper.Map<LogMigracionDto>(item)).ToList()
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(FindAll));
      }
    }
  }
}
