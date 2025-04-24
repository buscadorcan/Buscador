/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/EsquemaController: Controlador para formularios de esquemas en aplicativo
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Response;
using SharedApp.Dtos;
using DataAccess.Models;
using Core.Interfaces;

namespace WebApp.Controllers
{
    [Route("api/esquema")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class EsquemaController(
      IEsquemaService iEsquemaService,
      IMapper mapper
    ) : BaseController
    {
        private readonly IEsquemaService _iEsquemaService = iEsquemaService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// FindAll
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de EsquemaDto que representa todos los registros disponibles.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<EsquemaDto>>
                {
                    Result = _iEsquemaService.FindAll().Select(item => _mapper.Map<EsquemaDto>(item)).ToList()
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
        /// <param name="id">Identificador del registro a buscar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con un EsquemaDto correspondiente al registro encontrado.
        /// En caso de que el registro no exista, devuelve un mensaje de error adecuado.
        /// </returns>
        [Authorize]
        [HttpGet("{id:int}")]
        public IActionResult FindById(int id)
        {
            try
            {
                var record = _iEsquemaService.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                return Ok(new RespuestasAPI<EsquemaDto>
                {
                    Result = _mapper.Map<EsquemaDto>(record)
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
        /// <param name="id">Identificador del registro a actualizar.</param>
        /// <param name="dto">Objeto EsquemaDto con la información actualizada.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la actualización fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] EsquemaDto dto)
        {
            try
            {
                dto.IdEsquema = id;
                var record = _mapper.Map<Esquema>(dto);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iEsquemaService.Update(record)
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
        /// <param name="dto">Objeto EsquemaDto con la información del nuevo registro.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la creación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] EsquemaDto dto)
        {
            try
            {
                var record = _mapper.Map<Esquema>(dto);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iEsquemaService.Create(record)
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
        /// <param name="id">Identificador del registro a desactivar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la operación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Deactive(int id)
        {
            try
            {
                var record = _iEsquemaService.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                record.Estado = "X";

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iEsquemaService.Update(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Deactive));
            }
        }

        /// <summary>
        /// EliminarEsquemaVistaColumnaByIdEquemaVistaAsync
        /// </summary>
        /// <param name="esquemaRegistro">Objeto EsquemaVistaValidacionDto con la información del esquema a eliminar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la eliminación fue exitosa.
        /// </returns>
        [Authorize]
        [HttpDelete("validacion")]
        public IActionResult EliminarEsquemaVistaColumnaByIdEquemaVistaAsync([FromBody] EsquemaVistaValidacionDto esquemaRegistro)
        {
            try
            {
                var record = _iEsquemaService.GetEsquemaVistaColumnaByIdEquemaVistaAsync(esquemaRegistro.IdOna, esquemaRegistro.IdEsquema);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                record.Estado = "X";

                var isDeleted = _iEsquemaService.EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(record.IdEsquemaVista);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = isDeleted
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(EliminarEsquemaVistaColumnaByIdEquemaVistaAsync));
            }
        }

        /// <summary>
        /// UpdateEsquemaValidacion
        /// </summary>
        /// <param name="dto">Objeto EsquemaVistaValidacionDto con la información actualizada.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la actualización fue exitosa.
        /// </returns>
        [Authorize]
        [Route("validacion/actualizar")]
        [HttpPut]
        public IActionResult UpdateEsquemaValidacion([FromBody] EsquemaVistaValidacionDto dto)
        {
            try
            {
                var record = _mapper.Map<EsquemaVista>(dto);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iEsquemaService.UpdateEsquemaValidacion(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(UpdateEsquemaValidacion));
            }
        }

        /// <summary>
        /// CreateEsquemaValidacion
        /// </summary>
        /// <param name="dto">Objeto EsquemaVistaValidacionDto con la información de la validación a crear.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la creación fue exitosa.
        /// </returns>
        [Authorize]
        [Route("validacion")]
        [HttpPost]
        public IActionResult CreateEsquemaValidacion([FromBody] EsquemaVistaValidacionDto dto)
        {
            try
            {
                var record = _mapper.Map<EsquemaVista>(dto);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iEsquemaService.CreateEsquemaValidacion(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(CreateEsquemaValidacion));
            }
        }

        /// <summary>
        /// GuardarListaEsquemaVistaColumna
        /// </summary>
        /// <param name="listaEsquemaVistaColumna">Lista de objetos EsquemaVistaColumnaDto que representan las columnas a guardar.</param>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA) al que pertenece el esquema.</param>
        /// <param name="idEsquema">Identificador del esquema donde se almacenarán las columnas.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la operación de guardado fue exitosa.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [Route("vista/columna")]
        [HttpPost]
        public IActionResult GuardarListaEsquemaVistaColumna(
            [FromBody] List<EsquemaVistaColumnaDto> listaEsquemaVistaColumna,
            [FromQuery] int idOna,
            [FromQuery] int idEsquema)
        {
            try
            {
                var record = _mapper.Map<List<EsquemaVistaColumna>>(listaEsquemaVistaColumna);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iEsquemaService.GuardarListaEsquemaVistaColumna(record, idOna, idEsquema)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GuardarListaEsquemaVistaColumna));
            }
        }

        /// <summary>
        /// GetListaEsquemaByOna
        /// </summary>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de esquemas asociados al ONA especificado.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet("esquemas/{idOna}", Name = "GetListaEsquemaByOna")]
        public IActionResult GetListaEsquemaByOna(int idOna)
        {
            try
            {
                var result = _iEsquemaService.GetListaEsquemaByOna(idOna);
                return Ok(new RespuestasAPI<List<Esquema>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetListaEsquemaByOna));
            }
        }


    }
}

