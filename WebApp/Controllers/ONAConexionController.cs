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
        [HttpGet("onaconexion/{idOna:int}")]
        public IActionResult GetOnaConexionByOna(int idOna)
        {
            try
            {
                var result = _iRepo.GetOnaConexionByOnaAsync(idOna);

                return Ok(new RespuestasAPI<ONAConexionDto>
                {
                    Result = _mapper.Map<ONAConexionDto>(result)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetOnaConexionByOna));
            }
        }

        [Authorize]
        [HttpGet("test/{idOna:int}")]
        public IActionResult TestConexionByOna(int idOna)
        {
            try
            {
                var result = _iRepo.GetOnaConexionByOnaAsync(idOna);
                bool conexion = _iRepo.TestConnection(result);
                if (conexion) 
                { 
                    return Ok(conexion); 
                }
                else
                {
                    Console.WriteLine("No se que esta pasando");
                }
                return Ok(conexion);

                         
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetOnaConexionByOna));
            }
        }

        [Authorize]
        [HttpGet("migrar/{idOna:int}")]
        public IActionResult MigrarConexionByOna(int idOna)
        {
            try
            {
                var result = _iRepo.GetOnaConexionByOnaAsync(idOna);
                bool conexion = _iRepo.TestConnection(result);
                if (conexion)
                {
                    return Ok(conexion);
                }
                else
                {
                    Console.WriteLine("No se que esta pasando");
                }
                return Ok(conexion);


            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetOnaConexionByOna));
            }
        }
    }
}
