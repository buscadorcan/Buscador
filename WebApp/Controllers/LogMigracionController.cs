/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/LogMigracionController: Controlador para log de migración
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Response;
using DataAccess.Interfaces;
using SharedApp.Dtos;
using Core.Interfaces;

namespace WebApp.Controllers
{
  [Route(Routes.LOG_MIGRACION)]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public class LogMigracionController : BaseController
  {
        private readonly ILogMigracionService _logMigracionService;
        private readonly IMapper _mapper;

        public LogMigracionController(ILogMigracionService logMigracionService,
            IMapper mapper,
            ILogger<BaseController> logger)
            : base(logger)
        {
            this._logMigracionService = logMigracionService;
            this._mapper = mapper;
        }

        /// <summary>
        /// FindAll
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de LogMigracionDto representando los registros del log de migración.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
    [HttpGet]
    public IActionResult FindAll()
    {
      try
      {
        return Ok(new RespuestasAPI<List<LogMigracionDto>> {
          Result = _logMigracionService.FindAll().Select(item => _mapper.Map<LogMigracionDto>(item)).ToList()
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(FindAll));
      }
    }
  }
}
