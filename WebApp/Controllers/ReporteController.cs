using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Repositories.IRepositories;

namespace WebApp.Controllers
{
    [Route("api/reportevista")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class ReporteController(IReporteRepository iRepo, IMapper mapper) : BaseController
    {
        private readonly IReporteRepository _vhRepo = iRepo;
        private readonly IMapper _mapper = mapper;

        [Authorize]
        [HttpGet("acreditacion-ona")]
        public IActionResult ObtenerAcreditacionOna()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwAcreditacionOnaDto>>
                {
                    Result = _vhRepo.ObtenerVwAcreditacionOna().Select(item => _mapper.Map<VwAcreditacionOnaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerAcreditacionOna));
            }
        }

        [Authorize]
        [HttpGet("acreditacion-esquema")]
        public IActionResult ObtenerAcreditacionEsquema()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwAcreditacionEsquemaDto>>
                {
                    Result = _vhRepo.ObtenerVwAcreditacionEsquema().Select(item => _mapper.Map<VwAcreditacionEsquemaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerAcreditacionEsquema));
            }
        }

        [Authorize]
        [HttpGet("estado-esquema")]
        public IActionResult ObtenerEstadoEsquema()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwEstadoEsquemaDto>>
                {
                    Result = _vhRepo.ObtenerVwEstadoEsquema().Select(item => _mapper.Map<VwEstadoEsquemaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerEstadoEsquema));
            }
        }

        [Authorize]
        [HttpGet("oec-pais")]
        public IActionResult ObtenerOecPais()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwOecPaisDto>>
                {
                    Result = _vhRepo.ObtenerVwOecPais().Select(item => _mapper.Map<VwOecPaisDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerOecPais));
            }
        }

        [Authorize]
        [HttpGet("esquema-pais")]
        public IActionResult ObtenerEsquemaPais()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwEsquemaPaisDto>>
                {
                    Result = _vhRepo.ObtenerVwEsquemaPais().Select(item => _mapper.Map<VwEsquemaPaisDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerEsquemaPais));
            }
        }

        [Authorize]
        [HttpGet("oec-fecha")]
        public IActionResult ObtenerOecFecha()
        {
            try
            {
                return Ok(new RespuestasAPI<List<VwOecFechaDto>>
                {
                    Result = _vhRepo.ObtenerVwOecFecha().Select(item => _mapper.Map<VwOecFechaDto>(item)).ToList()
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(ObtenerOecFecha));
            }
        }
    }
}
