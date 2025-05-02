/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/HomologacionController: Controlador para funcionalidades de Homologación
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Response;
using SharedApp.Dtos;
using DataAccess.Models;
using Core.Interfaces;

namespace WebApp.Controllers
{
    [Route(Routes.HOMOLOGACION)]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class HomologacionController : BaseController
    {
        private readonly IHomologacionService _iHomologacionService;
        private readonly IMapper _mapper;

        public HomologacionController( IHomologacionService iHomologacionService, 
                                       IMapper mapper, 
                                       ILogger<BaseController> logger) : base(logger)
        {
            this._iHomologacionService = iHomologacionService;
            this._mapper = mapper;
        }

        /// <summary>
        /// FindByParent
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de HomologacionDto que representa las homologaciones encontradas.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet(Routes.FIND_BY_PARENT)]
        public IActionResult FindByParent()
        {
            try
            {
                return Ok(new RespuestasAPI<List<HomologacionDto>>
                {
                    Result = _iHomologacionService.FindByParent().Select(item => _mapper.Map<HomologacionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindByParent));
            }
        }

        /// <summary>
        /// FindById
        /// </summary>
        /// <param name="id">Identificador único de la homologación a buscar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con un HomologacionDto correspondiente al registro encontrado.
        /// En caso de que el registro no exista, devuelve un mensaje de error adecuado.
        /// </returns>
        [Authorize]
        [HttpGet("{id:int}")]
        public IActionResult FindById(int id)
        {
            try
            {
                var record = _iHomologacionService.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                return Ok(new RespuestasAPI<HomologacionDto>
                {
                    Result = _mapper.Map<HomologacionDto>(record)
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
        /// <param name="id">Identificador único de la homologación a actualizar.</param>
        /// <param name="dto">Objeto HomologacionDto con la información actualizada.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la actualización fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] HomologacionDto dto)
        {
            try
            {
                dto.IdHomologacion = id;
                dto.Estado = "A";

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iHomologacionService.Update(_mapper.Map<Homologacion>(dto))
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
        /// <param name="dto">Objeto HomologacionDto con la información de la nueva homologación.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la creación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] HomologacionDto dto)
        {
            try
            {
                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iHomologacionService.Create(_mapper.Map<Homologacion>(dto))
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Create));
            }
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="id">Identificador único de la homologación a desactivar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la operación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Deactive(int id)
        {
            try
            {
                var record = _iHomologacionService.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                record.Estado = "X";

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iHomologacionService.Update(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Deactive));
            }
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="codigoHomologacion">Código de homologación por el cual se filtrarán los registros.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de VwHomologacionDto que representan las homologaciones encontradas.
        /// </returns>
        [Authorize]
        [HttpGet(Routes.FIND_BY_CODIGO_HOMOLOGACION)]
        public IActionResult FindByCodigoHomologacion(string codigoHomologacion)
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwHomologacionDto>>
                {
                    Result = _iHomologacionService.ObtenerVwHomologacionPorCodigo(codigoHomologacion)
                        .Select(item => _mapper.Map<VwHomologacionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindByCodigoHomologacion));
            }
        }

        /// <summary>
        /// FindByAll
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de HomologacionDto representando todas las homologaciones.
        /// </returns>
        [Authorize]
        [HttpGet(Routes.FIND_BY_ALL_HOMOLOGACION)]
        public IActionResult FindByAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<HomologacionDto>>
                {
                    Result = _iHomologacionService.FindByAll().Select(item => _mapper.Map<HomologacionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindByAll));
            }
        }

    }
}
