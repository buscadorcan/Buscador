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


    }
}
