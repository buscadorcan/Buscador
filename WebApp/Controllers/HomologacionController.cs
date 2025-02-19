using WebApp.Models;
using WebApp.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Models;
using SharedApp.Models.Dtos;

namespace WebApp.Controllers
{
    [Route("api/homologacion")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class HomologacionController(
      IHomologacionRepository iRepo,
      IMapper mapper
    ) : BaseController
    {
        private readonly IHomologacionRepository _iRepo = iRepo;
        private readonly IMapper _mapper = mapper;

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByParent: Obtiene la lista de homologaciones basadas en un identificador de padre.
         */
        [HttpGet("findByParent")]
        public IActionResult FindByParent()
        {
            try
            {
                return Ok(new RespuestasAPI<List<HomologacionDto>>
                {
                    Result = _iRepo.FindByParent().Select(item => _mapper.Map<HomologacionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindByParent));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindById: Obtiene una homologación específica por su ID.
         */
        [Authorize]
        [HttpGet("{id:int}")]
        public IActionResult FindById(int id)
        {
            try
            {
                var record = _iRepo.FindById(id);

                if (record == null)
                {
                    return NotFoundResponse("Reguistro no encontrado");
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Update: Actualiza los datos de una homologación existente.
         */
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
                    IsSuccess = _iRepo.Update(_mapper.Map<Homologacion>(dto))
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Update));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Create: Crea una nueva homologación en el sistema.
         */
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] HomologacionDto dto)
        {
            try
            {
                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iRepo.Create(_mapper.Map<Homologacion>(dto))
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Create));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/Deactive: Desactiva una homologación estableciendo su estado en "X".
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
         * WebApp/FindByCodigoHomologacion: Obtiene homologaciones filtradas por código de homologación.
         */
        [Authorize]
        [HttpGet("findByCodigoHomologacion/{codigoHomologacion}")]
        public IActionResult FinfindByCodigoHomologaciondByParent(string codigoHomologacion)
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwHomologacionDto>>
                {
                    Result = _iRepo.ObtenerVwHomologacionPorCodigo(codigoHomologacion).Select(item => _mapper.Map<VwHomologacionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FinfindByCodigoHomologaciondByParent));
            }
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/FindByAll: Obtiene todas las homologaciones registradas en el sistema.
         */
        [Authorize]
        [HttpGet("FindByAll")]
        public IActionResult FindByAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<HomologacionDto>>
                {
                    Result = _iRepo.FindByAll().Select(item => _mapper.Map<HomologacionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindByParent));
            }
        }
    }
}
