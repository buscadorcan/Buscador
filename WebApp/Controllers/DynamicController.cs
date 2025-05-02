/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/DynamicController: Controlador para formularios de aplicativo
using Microsoft.AspNetCore.Mvc;
using SharedApp.Response;
using Microsoft.AspNetCore.Authorization;
using SharedApp.Dtos;
using Core.Interfaces;

namespace WebApp.Controllers
{
  [Route(Routes.DYNAMIC)]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public class DynamicController : BaseController
  {
    private readonly IDynamicService _dynamicService;
    public DynamicController(IDynamicService dynamicService, ILogger<BaseController> logger) : base(logger)
    {
            _dynamicService = dynamicService;
    }

        /// <summary>
        /// GetProperties
        /// </summary>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <param name="viewName">Nombre de la vista para la cual se desean obtener las propiedades de las columnas.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de PropiedadesTablaDto que representa las propiedades de las columnas.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet(Routes.GETPROPERTIES, Name = "getProperties")]
        public IActionResult GetProperties(int idOna, string viewName)
        {
            try
            {
                var result = _dynamicService.GetProperties(idOna, viewName);
                return Ok(new RespuestasAPI<List<PropiedadesTablaDto>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetProperties));
            }
        }

        /// <summary>
        /// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
        /// WebApp/GetValueColumna: Obtiene el valor de una columna específica dentro de una vista para un ONA determinado.
        /// Este método permite recuperar el valor de una columna específica en una vista dada dentro del contexto de un ONA.
        /// </summary>
        /// <param name="idONA">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <param name="valueColumn">Nombre de la columna cuyo valor se desea obtener.</param>
        /// <param name="viewName">Nombre de la vista de la cual se extraerá el valor de la columna.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de PropiedadesTablaDto que representa los valores obtenidos.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet(Routes.GETVALUECOLUMNA, Name = "GetValueColumna")]
        public IActionResult GetValueColumna(int idONA, string valueColumn, string viewName)
        {
            try
            {
                var result = _dynamicService.GetValueColumna(idONA, valueColumn, viewName);
                return Ok(new RespuestasAPI<List<PropiedadesTablaDto>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetValueColumna));
            }
        }

        /// <summary>
        /// GetViewNames
        /// </summary>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de nombres de vistas en formato de cadena.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet("{idOna}", Name = "getViewNames")]
        public IActionResult GetViewNames(int idOna)
        {
            try
            {
                var result = _dynamicService.GetViewNames(idOna);
                return Ok(new RespuestasAPI<List<string>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetViewNames));
            }
        }

        /// <summary>
        /// GetListaValidacionEsquema
        /// </summary>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <param name="idEsquema">Identificador del esquema para el cual se desean obtener las validaciones.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con una lista de EsquemaVistaDto que representa las validaciones disponibles.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet(Routes.GETLISTAVALIDACIONESQUEMA, Name = "GetListaValidacionEsquema")]
        public IActionResult GetListaValidacionEsquema(int idOna, int idEsquema)
        {
            try
            {
                var result = _dynamicService.GetListaValidacionEsquema(idOna, idEsquema);
                return Ok(new RespuestasAPI<List<EsquemaVistaDto>> { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(GetListaValidacionEsquema));
            }
        }

        /// <summary>
        /// TestConnection
        /// </summary>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA).</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la conexión se estableció correctamente o no.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpGet(Routes.TEST)]
        public IActionResult TestConnection(int idOna)
        {
            try
            {
                var conexion = _dynamicService.GetConexion(idOna);
                if (conexion == null)
                {
                    return NotFound(new { Message = "Conexión no encontrada." });
                }

                var isConnected = _dynamicService.TestDatabaseConnection(conexion);

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

        /// <summary>
        /// MigrarConexion
        /// </summary>
        /// <param name="idOna">Identificador del Organismo Nacional de Acreditación (ONA) cuya conexión será migrada.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la migración de la conexión se realizó con éxito o no.
        /// En caso de error, maneja la excepción y devuelve un mensaje adecuado.
        /// </returns>
        [Authorize]
        [HttpPost(Routes.MIGRAR)]
        public async Task<IActionResult> MigrarConexion(int idOna)
        {
            try
            {
                bool resultado = await _dynamicService.MigrarConexionAsync(idOna);

                return Ok(new
                {
                    IsSuccess = resultado,
                    Message = resultado ? "Conexión migrada correctamente." : "No se pudo migrar la conexión."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Success = false, Message = ex.Message });
            }
        }


    }
}
