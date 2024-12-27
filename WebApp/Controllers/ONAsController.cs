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
        [Authorize]
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                return Ok(new RespuestasAPI<List<ONADto>>
                {
                    Result = _iRepo.FindAll().Select(item => _mapper.Map<ONADto>(item)).ToList()
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

                return Ok(new RespuestasAPI<ONADto>
                {
                    Result = _mapper.Map<ONADto>(record)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindById));
            }
        }
        [Authorize]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] ONADto dto)
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
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] ONADto dto)
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
