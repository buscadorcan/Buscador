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
    [Route("api/ona")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ONAsController(IONARepository iRepo, IMapper mapper) : BaseController
    {
        private readonly IONARepository _iRepo = iRepo;
        private readonly IMapper _mapper = mapper;

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAll: Obtiene todos los registros de ONAs.
         */
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<OnaDto>>
                {
                    Result = _iRepo.FindAll().Select(item => _mapper.Map<OnaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAll));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetListByONAsAsync: Obtiene la lista de ONAs asociados a un ONA específico.
         */
        [Authorize]
        [HttpGet("Lista/{idOna:int}")]
        public IActionResult GetListByONAsAsync(int idOna)
        {
            try
            {
                return Ok(new RespuestasAPI<List<OnaDto>>
                {
                    Result = _iRepo.GetListByONAsAsync(idOna).Select(item => _mapper.Map<OnaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetListByONAsAsync));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindAllPais: Obtiene la lista de países disponibles en el sistema.
         */
        [Authorize]
        [HttpGet("paises")]
        public IActionResult FindAllPais()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwPaisDto>>
                {
                    Result = _iRepo.FindAllPaises().Select(item => _mapper.Map<VwPaisDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAllPais));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Obtiene un ONA específico por su ID.
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza los datos de un ONA existente.
         */
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
         * WebApp/Create: Crea un nuevo ONA en el sistema.
         */
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] OnaDto dto)
        {
            try
            {
                var record = _mapper.Map<ONA>(dto);

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
         * WebApp/Deactive: Desactiva un ONA estableciendo su estado en "X".
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
