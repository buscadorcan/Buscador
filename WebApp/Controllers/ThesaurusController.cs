using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Service.IService;
using AutoMapper;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("api/thesaurus")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ThesaurusController(IThesaurusService thesaurusService, IMapper mapper) : BaseController
    {
        private readonly IThesaurusService _thesaurusService = thesaurusService;
        private readonly IMapper _mapper = mapper;


        //[Authorize]
        [HttpGet("obtener/thesaurus")]
        public IActionResult ObtenerThesaurus()
        {
            try
            {
                return Ok(new RespuestasAPI<ThesaurusDto>
                {
                    Result = _mapper.Map<ThesaurusDto>(_thesaurusService.ObtenerThesaurus())
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerThesaurus));
            }
        }

        //[Authorize]
        [HttpPost("agregar/expansion")]
        public IActionResult AgregarExpansion([FromBody] List<string> sinonimos)
        {
            try
            {
                return Ok(new RespuestasAPI<string>
                {
                    Result = _thesaurusService.AgregarExpansion(sinonimos)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AgregarExpansion));
            }
        }

        //[Authorize]
        [HttpPost("actualizar/expansion")]
        public IActionResult ActualizarExpansion([FromBody] List<ExpansionDto> expansions)
        {
            try
            {
                var exp = _mapper.Map<List<Expansion>>(expansions);
                return Ok(new RespuestasAPI<string>
                {
                    Result = _thesaurusService.ActualizarExpansion(exp)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AgregarExpansion));
            }
        }

        //[Authorize]
        [HttpGet("agregar/expansion/{expansionExistente}/sub/{nuevoSub}")]
        public IActionResult AgregarSubExpansion([FromRoute] string expansionExistente, string nuevoSub)
        {
            try
            {
                return Ok(new RespuestasAPI<string>
                {
                    Result = _thesaurusService.AgregarSubAExpansion(expansionExistente, nuevoSub)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AgregarSubExpansion));
            }
        }

        [HttpGet("ejecutar/bat")]
        public IActionResult EjecutarBat()
        {
            try
            {
                return Ok(new RespuestasAPI<string>
                {
                    Result = _thesaurusService.EjecutarArchivoBat()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(AgregarSubExpansion));
            }
        }
    }
}
