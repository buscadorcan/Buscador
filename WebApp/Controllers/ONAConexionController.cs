/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/ONAConexionController: Controlador para funcionalidad de Ona conexión
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Response;
using DataAccess.Interfaces;
using SharedApp.Dtos;
using DataAccess.Models;
using Core.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/conexion")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ONAConexionController(IONAConexionService oNAConexionService, IMapper mapper) : BaseController
    {
        private readonly IONAConexionService _oNAConexionService = oNAConexionService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// FindAll
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de ONAConexionDto representando las conexiones registradas.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<ONAConexionDto>>
                {
                    Result = _oNAConexionService.FindAll().Select(item => _mapper.Map<ONAConexionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAll));
            }
        }

        /// <summary>
        /// GetOnaConexionByOnaListAsync
        /// </summary>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de ONAConexionDto representando las conexiones encontradas.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet("ListaOna/{idOna:int}")]
        public IActionResult GetOnaConexionByOnaListAsync(int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<ONAConexionDto>>
                {
                    Result = _oNAConexionService.GetOnaConexionByOnaListAsync(idOna).Select(item => _mapper.Map<ONAConexionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetOnaConexionByOnaListAsync));
            }
        }

        /// <summary>
        /// FindById
        /// </summary>
        /// <param name="id">Identificador único de la conexión ONA.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con un ONAConexionDto correspondiente al registro encontrado.
        /// En caso de que el registro no exista, devuelve un mensaje de error adecuado.
        /// </returns>
        [Authorize]
        [HttpGet("{id:int}")]
        public IActionResult FindById(int id)
        {
            try
            {
                var record = _oNAConexionService.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                return Ok(new RespuestasAPI<ONAConexionDto>
                {
                    Result = _mapper.Map<ONAConexionDto>(record)
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
        /// <param name="id">Identificador único de la conexión ONA a actualizar.</param>
        /// <param name="dto">Objeto ONAConexionDto con la información actualizada.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la actualización fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ONAConexionDto dto)
        {
            try
            {
                dto.IdONA = id;
                var homologacion = _mapper.Map<ONAConexion>(dto);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _oNAConexionService.Update(homologacion)
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
        /// <param name="dto">Objeto ONAConexionDto con la información de la nueva conexión ONA.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la creación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] ONAConexionDto dto)
        {
            try
            {
                var record = _mapper.Map<ONAConexion>(dto);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _oNAConexionService.Create(record)
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
        /// <param name="id">Identificador único de la conexión ONA a desactivar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la operación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Deactive(int id)
        {
            try
            {
                var record = _oNAConexionService.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                record.Estado = "X";

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _oNAConexionService.Update(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Deactive));
            }
        }


    }
}
