using WebApp.Models;
using WebApp.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Models.Dtos;
using SharedApp.Models;

namespace WebApp.Controllers
{
    [Route("api/conexion")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ONAConexionController(IONAConexionRepository iRepo, IMapper mapper) : BaseController
    {
        private readonly IONAConexionRepository _iRepo = iRepo;
        private readonly IMapper _mapper = mapper;

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene todas las conexiones ONA registradas.
         */
        [Authorize]
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<ONAConexionDto>>
                {
                    Result = _iRepo.FindAll().Select(item => _mapper.Map<ONAConexionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAll));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetOnaConexionByOnaListAsync: Obtiene la lista de conexiones ONA asociadas a un ONA específico.
         */
        [Authorize]
        [HttpGet("ListaOna/{idOna:int}")]
        public IActionResult GetOnaConexionByOnaListAsync(int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<ONAConexionDto>>
                {
                    Result = _iRepo.GetOnaConexionByOnaListAsync(idOna).Select(item => _mapper.Map<ONAConexionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetOnaConexionByOnaListAsync));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Obtiene una conexión ONA específica por su ID.
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza los datos de una conexión ONA existente.
         */
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
                    IsSuccess = _iRepo.Update(homologacion)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Update));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea una nueva conexión ONA en el sistema.
         */
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] ONAConexionDto dto)
        {
            try
            {
                var record = _mapper.Map<ONAConexion>(dto);

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
         * WebApp/Deactive: Desactiva una conexión ONA estableciendo su estado en "X".
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

    }
}
