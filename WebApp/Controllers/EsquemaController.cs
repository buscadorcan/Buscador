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
        [Authorize]
        [HttpDelete("validacion/{id:int}")]
        public IActionResult EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(int id)
        {
            try
            {
                var record = _iRepo.GetEsquemaVistaColumnaByIdEquemaVistaAsync(id);

                if (record == null)
                {
                    return NotFoundResponse("Reguistro no encontrado");
                }

                record.Estado = "X";

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iRepo.EliminarEsquemaVistaColumnaByIdEquemaVistaAsync(id)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Deactive));
            }
        }
        
        [Authorize]
        [HttpPut("validacion/{id:int}")]
        public IActionResult UpdateEsquemaValidacion(int id, [FromBody] EsquemaVistaValidacionDto dto)
        {
            try
            {
                dto.IdEsquemaVista = id;
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
        [Authorize]
        [Route("vista/columna")]
        [HttpPost]
        public IActionResult GuardarListaEsquemaVistaColumna([FromBody] List<EsquemaVistaColumnaDto> listaEsquemaVistaColumna)
        {
            try
            {
                var record = _mapper.Map<List<EsquemaVistaColumna>>(listaEsquemaVistaColumna);

                return Ok(new RespuestasAPI<bool>
                {
                    IsSuccess = _iRepo.GuardarListaEsquemaVistaColumna(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GuardarListaEsquemaVistaColumna));
            }
        }

        [Authorize]
        [HttpGet("esquemas/{idOna}", Name = "GetListaEsquemaByOna")]
        public IActionResult GetListaEsquemaByOna(int idOna)
        {
            try
            {
                var result = _iRepo.GetListaEsquemaByOna(idOna);
                return Ok(new RespuestasAPI<List<EsquemaVistaOnaDto>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetListaEsquemaByOna));
            }
        }


    }
}

