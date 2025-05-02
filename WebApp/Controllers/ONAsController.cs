/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/ONAsController: Controlador para funcionalidad de Onas
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
    [Route(Routes.ONA)]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ONAsController : BaseController
    {
        private readonly IONAService _oNAService;
        private readonly IMapper _mapper;

        public ONAsController(IONAService oNAService, IMapper mapper, ILogger<BaseController> logger): base(logger)
        {
            this._oNAService = oNAService;
            this._mapper = mapper;
        }

        /// <summary>
        /// FindAll
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de OnaDto representando los ONAs registrados.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<OnaDto>>
                {
                    Result = _oNAService.FindAll().Select(item => _mapper.Map<OnaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAll));
            }
        }

        /// <summary>
        /// GetListByONAsAsync
        /// </summary>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de OnaDto representando las asociaciones encontradas.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet(Routes.LISTA_ONA_BY_ID)]
        public IActionResult GetListByONAsAsync(int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<OnaDto>>
                {
                    Result = _oNAService.GetListByONAsAsync(idOna).Select(item => _mapper.Map<OnaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetListByONAsAsync));
            }
        }

        /// <summary>
        /// FindAllPais
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de VwPaisDto representando los países disponibles.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet(Routes.PAISES)]
        public IActionResult FindAllPais()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwPaisDto>>
                {
                    Result = _oNAService.FindAllPaises().Select(item => _mapper.Map<VwPaisDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAllPais));
            }
        }

        /// <summary>
        /// FindById
        /// </summary>
        /// <param name="id">Identificador único del ONA.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con un OnaDto correspondiente al registro encontrado.
        /// En caso de que el registro no exista, devuelve un mensaje de error adecuado.
        /// </returns>
        [HttpGet("{id:int}")]
        public IActionResult FindById(int id)
        {
            try
            {
                var record = _oNAService.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                return Ok(new RespuestasAPI<OnaDto>
                {
                    Result = _mapper.Map<OnaDto>(record)
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
        /// <param name="id">Identificador único del ONA a actualizar.</param>
        /// <param name="dto">Objeto OnaDto con la información actualizada.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la actualización fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] OnaDto dto)
        {
            try
            {
                dto.IdONA = id;
                var homologacion = _mapper.Map<ONA>(dto);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _oNAService.Update(homologacion)
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
        /// <param name="dto">Objeto OnaDto con la información del nuevo ONA.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la creación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] OnaDto dto)
        {
            try
            {
                var record = _mapper.Map<ONA>(dto);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _oNAService.Create(record)
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
        /// <param name="id">Identificador único del ONA a desactivar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la operación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Deactive(int id)
        {
            try
            {
                var record = _oNAService.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                record.Estado = "X";

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _oNAService.Update(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Deactive));
            }
        }

    }
}
