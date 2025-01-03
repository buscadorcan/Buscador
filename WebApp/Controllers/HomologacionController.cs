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
        [Authorize]
        [HttpGet("findByParent/{valor}")]
        public IActionResult FindByParent(int valor)
        {
            try
            {
                return Ok(new RespuestasAPI<List<HomologacionDto>>
                {
                    Result = _iRepo.FindByParent(valor).Select(item => _mapper.Map<HomologacionDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindByParent));
            }
        }
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
        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] HomologacionDto dto)
        {
            try
            {
                dto.IdHomologacion = id;

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
