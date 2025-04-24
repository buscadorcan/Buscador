/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/MenuController: Controlador para menú
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Response;
using DataAccess.Interfaces;
using SharedApp.Dtos;
using DataAccess.Models;
using Core.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/menu")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class MenuController(IMenuService menuService, IMapper mapper) : BaseController
    {
        private readonly IMenuService _menuService = menuService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// FindAll
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de MenuDto representando los registros del menú.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<MenuRolDto>>
                {
                    Result = _menuService.FindAll().Select(item => _mapper.Map<MenuRolDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAll));
            }
        }

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
                var record = _menuService.FindDataById(idHRol, idHMenu);
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
        /// Update
        /// </summary>
        /// <param name="idHRol">Identificador del rol asociado al menú.</param>
        /// <param name="idHMenu">Identificador del menú a actualizar.</param>
        /// <param name="dto">Objeto MenuDto con la información actualizada.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la actualización fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPut("{idHRol:int}/{idHMenu:int}")]
        public IActionResult Update(int idHRol, int idHMenu, [FromBody] MenuRolDto dto)
        {
            try
            {
                MenuRol menuRol = new MenuRol
                {
                    IdMenuRol = dto.IdMenuRol,
                    IdHRol = dto.IdHRol,
                    IdHMenu = dto.IdHMenu,
                    Estado = dto.Estado,
                    FechaCreacion = dto.FechaCreacion
                };

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _menuService.Update(menuRol)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Update));
            }
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="dto">Objeto MenuDto con la información del nuevo registro del menú.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la creación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] MenuRolDto dto)
        {
            try
            {
                var record = _mapper.Map<MenuRol>(dto);
                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _menuService.Create(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Create));
            }
        }

        /// <summary>
        /// Deactive
        /// </summary>
        /// <param name="idHRol">Identificador del rol asociado al menú.</param>
        /// <param name="idHMenu">Identificador del menú a desactivar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la operación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpDelete("{idHRol:int}/{idHMenu:int}")]
        public IActionResult Deactive(int idHRol, int idHMenu)
        {
            try
            {
                var record = _menuService.FindById(idHRol, idHMenu);
                record.Estado = record.Estado == "A" ? "X" : "A";
                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }
                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _menuService.Update(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Deactive));
            }
        }
        /// <summary>
        /// Deactive
        /// </summary>
        /// <param name="idHomologacionRol">Identificador del rol en homologación.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con un MenuPaginaDto correspondiente al registro encontrado.
        /// En caso de que el registro no exista, devuelve un mensaje de error adecuado.
        /// </returns>
        [HttpGet("{idHomologacionRol:int}", Name = "menus")]
        public IActionResult ObtenerMenusPendingConfig(int idHomologacionRol)
        {
            try
            {
                return Ok(new RespuestasAPI<List<MenuPaginaDto>>
                {
                    Result = _menuService.ObtenerMenusPendingConfig(idHomologacionRol).Select(item => _mapper.Map<MenuPaginaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerMenusPendingConfig));
            }
        }
    }
}
