using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Models;
using WebApp.Repositories.IRepositories;

namespace WebApp.Controllers
{
    [Route("api/eventtracking")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class EventTrackingController(IEventTrackingRepository iRepo, IMapper mapper) : BaseController
    {
        private readonly IEventTrackingRepository _iRepo = iRepo;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// FindById
        /// </summary>
        /// <param name="idHRol">Identificador del rol asociado al menú.</param>
        /// <param name="idHMenu">Identificador del menú.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con un MenuDto correspondiente al registro encontrado.
        /// En caso de que el registro no exista, devuelve un mensaje de error adecuado.
        /// </returns>
        [HttpGet("{idHRol:int}/{idHMenu:int}")]
        public IActionResult FindById(int idHRol, int idHMenu)
        {
            try
            {
                var record = _iRepo.FindDataById(idHRol, idHMenu);
                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }
                return Ok(new RespuestasAPI<MenuRolDto>
                {
                    Result = _mapper.Map<MenuRolDto>(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindById));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("EventUserAll")]
        public IActionResult GetEventUserAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwEventUserAllDto>>
                {
                    Result = _iRepo.GetEventUserAll().Select(item => _mapper.Map<VwEventUserAllDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetEventUserAll));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <param name="fini"></param>
        /// <param name="ffin"></param>
        /// <returns></returns>
        [HttpGet("Even")]
        public IActionResult GetEventAll([FromQuery] string report, [FromQuery] DateOnly fini, [FromQuery] DateOnly ffin)
        {
            try
            {
                return Ok(new RespuestasAPI<List<EventUserDto>>
                {
                    Result = _iRepo.GetEventAll(report, fini, ffin).Select(item => _mapper.Map<EventUserDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetEventUserAll));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fini"></param>
        /// <param name="ffin"></param>
        /// <returns></returns>
        [HttpDelete("DeleteEven/{fini}/{ffin}")]
        public IActionResult DeleteEventAll(DateOnly fini, DateOnly ffin)
        {
            try
            {
                return Ok(new RespuestasAPI<bool>
                {
                    Result = _iRepo.DeleteEventAll(fini, ffin)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetEventUserAll));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codigoEvento"></param>
        /// <returns></returns>
        [HttpDelete("DeleteEvenById/{codigoEvento}")]
        public IActionResult DeleteEventById(int codigoEvento)
        {
            try
            {
                return Ok(new RespuestasAPI<bool>
                {
                    Result = _iRepo.DeleteEventById(codigoEvento)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetEventUserAll));
            }
        }

    }
}
