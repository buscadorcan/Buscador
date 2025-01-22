using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using WebApp.Repositories;

namespace WebApp.Controllers
{
  [Route("api/vistas")]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public class DynamicController : BaseController
  {
    private readonly IDynamicRepository _vhRepo;
    public DynamicController(IDynamicRepository vhRepo)
    {
      _vhRepo = vhRepo;
    }
    [Authorize]
    [HttpGet("columns/{idOna}/{viewName}", Name = "getProperties")]
    public IActionResult GetProperties(int idOna, string viewName)
    {
      try
      {
        var result = _vhRepo.GetProperties(idOna, viewName);
        return Ok(new RespuestasAPI<List<PropiedadesTablaDto>> { Result = result });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(GetProperties));
      }
    }
    
    [Authorize]
    [HttpGet("columns/{idOna}/{valueColumn}/{viewName}", Name = "GetValueColumna")]
    public IActionResult GetValueColumna(int idONA, string valueColumn, string viewName)    
    {
        try
        {
            var result = _vhRepo.GetValueColumna(idONA, valueColumn, viewName);
            return Ok(new RespuestasAPI<List<PropiedadesTablaDto>> { Result = result });
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(GetValueColumna));
        }
    }

    [Authorize]
    [HttpGet("{idOna}", Name = "getViewNames")]
    public IActionResult GetViewNames(int idOna)
    {
      try
      {
        var result = _vhRepo.GetViewNames(idOna);
        return Ok(new RespuestasAPI<List<string>> { Result = result });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(GetViewNames));
      }
    }

        //[Authorize]
        //[HttpGet("validacion/{idOna}/{idEsquemaVista}", Name = "GetListaValidacionEsquema")]
        //public IActionResult GetListaValidacionEsquema(int idOna, int idEsquemaVista)
        //{
        //    try
        //    {
        //        var result = _vhRepo.GetListaValidacionEsquema(idOna, idEsquemaVista);
        //        return Ok(new RespuestasAPI<List<EsquemaVistaDto>> { Result = result });
        //    }
        //    catch (Exception e)
        //    {
        //        return HandleException(e, nameof(GetListaValidacionEsquema));
        //    }
        //}
        [Authorize]
        [HttpGet("validacion/{idOna}/{idEsquema}", Name = "GetListaValidacionEsquema")]
        public IActionResult GetListaValidacionEsquema(int idOna, int idEsquema)
        {
            try
            {
                var result = _vhRepo.GetListaValidacionEsquema(idOna, idEsquema);
                return Ok(new RespuestasAPI<List<EsquemaVistaDto>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetListaValidacionEsquema));
            }
        }

        [Authorize]
        [HttpGet("test/{idOna:int}")]
        public IActionResult TestConnection(int idOna)
        {
            try
            {
                // Obtener la conexión desde el repositorio
                var conexion = _vhRepo.GetConexion(idOna);
                if (conexion == null)
                {
                    return NotFound(new { Message = "Conexión no encontrada." });
                }

                // Probar la conexión
                var isConnected = _vhRepo.TestDatabaseConnection(conexion);

                return Ok(new
                {
                    IsSuccess = isConnected,
                    Message = isConnected ? "Conexión establecida correctamente." : "No se pudo establecer la conexión."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error: {ex.Message}" });
            }
        }

        [Authorize]
        [HttpPost("migrar/{idOna:int}")]
        public async Task<IActionResult> MigrarConexion(int idOna)
        {
            try
            {
                bool resultado = await _vhRepo.MigrarConexionAsync(idOna);

                if (resultado)
                {
                    return Ok(new { Success = true, Message = "Migración completada con éxito." });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "La migración no se pudo completar." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = ex.Message });
            }
        }
    }
}
