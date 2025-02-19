using WebApp.Models;
using WebApp.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using MySqlX.XDevAPI.Common;

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
      IEsquemaRepository iRepo,
      IMapper mapper
    ) : BaseController
    {
        private readonly IEsquemaRepository _iRepo = iRepo;
        private readonly IMapper _mapper = mapper;

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene todos los registros del esquema.
         */
        [Authorize]
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<EsquemaDto>>
                {
                    Result = _iRepo.FindAll().Select(item => _mapper.Map<EsquemaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAll));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Busca un registro en el esquema por su ID.
         */
        [Authorize]
        [HttpGet("{id:int}")]
        public IActionResult FindById(int Id)
        {
            try
            {
                var record = _iRepo.FindById(Id);

                if (record == null)
                {
                    return NotFoundResponse("Reguistro no encontrado");
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza un registro en el esquema.
         */
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
                    IsSuccess = _iRepo.Update(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Update));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea un nuevo registro en el esquema.
         */
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] EsquemaDto dto)
        {
            try
            {
                var record = _mapper.Map<Esquema>(dto);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iRepo.Create(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Create));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Deactive: Desactiva un registro en el esquema cambiando su estado a "X".
         */
        [Authorize]
        [HttpDelete("{id:int}")]
        public IActionResult Deactive(int id)
        {
            try
            {
                var record = _iRepo.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Reguistro no encontrado");
                }

                record.Estado = "X";

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iRepo.Update(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Deactive));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/EliminarEsquemaVistaColumnaByIdEquemaVistaAsync: Elimina un esquema de vista columna basado en su ID de esquema y ONA.
         */
        [Authorize]
        [HttpDelete("validacion")]
        public IActionResult EliminarEsquemaVistaColumnaByIdEquemaVistaAsync([FromBody] EsquemaVistaValidacionDto esquemaRegistro)
        {
            try
            {
                // Buscar el registro basado en idOna e idEsquema del objeto recibido
                var record = _iRepo.GetEsquemaVistaColumnaByIdEquemaVistaAsync(esquemaRegistro.IdOna, esquemaRegistro.IdEsquema);

                if (record == null)
                {
                    return NotFoundResponse("Registro no encontrado");
                }

                // Actualizar el estado del registro a 'X'
                record.Estado = "X";

                var isDeleted = _iRepo.EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(record.IdEsquemaVista);

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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/UpdateEsquemaValidacion: Actualiza una validación de esquema.
         */
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
                    IsSuccess = _iRepo.UpdateEsquemaValidacion(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(UpdateEsquemaValidacion));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/CreateEsquemaValidacion: Crea una nueva validación de esquema.
         */
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
                    IsSuccess = _iRepo.CreateEsquemaValidacion(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(CreateEsquemaValidacion));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GuardarListaEsquemaVistaColumna: Guarda una lista de columnas en un esquema de vista.
         */
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

                // Puedes usar idOna y idEsquema en tu lógica según sea necesario
                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iRepo.GuardarListaEsquemaVistaColumna(record, idOna, idEsquema)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GuardarListaEsquemaVistaColumna));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetListaEsquemaByOna: Obtiene la lista de esquemas asociados a un ONA específico.
         */
        [Authorize]
        [HttpGet("esquemas/{idOna}", Name = "GetListaEsquemaByOna")]
        public IActionResult GetListaEsquemaByOna(int idOna)
        {
            try
            {
                var result = _iRepo.GetListaEsquemaByOna(idOna);
                return Ok(new RespuestasAPI<List<Esquema>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetListaEsquemaByOna));
            }
        }


    }
}

