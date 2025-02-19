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

    /* 
     * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/GetProperties: Obtiene las propiedades de las columnas de una vista espec�fica para un ONA determinado.
     */
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

    /* 
     * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/GetValueColumna: Obtiene el valor de una columna espec�fica dentro de una vista para un ONA determinado.
     */
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

    /* 
     * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/GetViewNames: Obtiene los nombres de las vistas asociadas a un ONA espec�fico.
     */
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

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/GetListaValidacionEsquema: Obtiene la lista de validaciones para un esquema espec�fico dentro de un ONA.
         */
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

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/TestConnection: Prueba la conexi�n a la base de datos de un ONA determinado.
         */
        [Authorize]
        [HttpGet("test/{idOna:int}")]
        public IActionResult TestConnection(int idOna)
        {
            try
            {
                // Obtener la conexi�n desde el repositorio
                var conexion = _vhRepo.GetConexion(idOna);
                if (conexion == null)
                {
                    return NotFound(new { Message = "Conexi�n no encontrada." });
                }

                // Probar la conexi�n
                var isConnected = _vhRepo.TestDatabaseConnection(conexion);

                return Ok(new
                {
                    IsSuccess = isConnected,
                    Message = isConnected ? "Conexi�n establecida correctamente." : "No se pudo establecer la conexi�n."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error: {ex.Message}" });
            }
        }

        /* 
         * Copyright � SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/MigrarConexion: Migra la conexi�n de un ONA espec�fico de forma as�ncrona.
         */
        [Authorize]
        [HttpPost("migrar/{idOna:int}")]
        public async Task<IActionResult> MigrarConexion(int idOna)
        {
            try
            {
                bool resultado = await _vhRepo.MigrarConexionAsync(idOna);

                return Ok(new
                {
                    IsSuccess = resultado,
                    Message = resultado ? "Conexi�n establecida correctamente." : "No se pudo establecer la conexi�n."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = ex.Message });
            }
        }
    }
}
