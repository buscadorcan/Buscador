using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using System.Net;

namespace WebApp.Controllers
{
    /// <summary>
    /// Clase base para los controladores. Proporciona métodos comunes para manejar respuestas HTTP y excepciones.
    /// </summary>
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Maneja excepciones no controladas y devuelve una respuesta de error con estado 500 (Internal Server Error).
        /// </summary>
        /// <param name="e">La excepción capturada.</param>
        /// <param name="methodName">El nombre del método donde ocurrió la excepción.</param>
        /// <returns>
        /// Un objeto <see cref="IActionResult"/> con el código de estado 500 y un mensaje de error estándar.
        /// </returns>
        protected IActionResult HandleException(Exception e, string methodName)
        {
            try
            {
                var logger = (ILogger<BaseController>?)HttpContext.RequestServices.GetService(typeof(ILogger<BaseController>));
                if (logger != null)
                {
                    logger.LogError(e, $"Error en {methodName}");
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new RespuestasAPI<object>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Error en el servidor" },
                    Result = new { }
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        /// <summary>
        /// Devuelve una respuesta de solicitud incorrecta con estado 400 (Bad Request).
        /// </summary>
        /// <param name="message">El mensaje de error que se enviará al cliente.</param>
        /// <returns>
        /// Un objeto <see cref="IActionResult"/> con el código de estado 400 y un mensaje de error.
        /// </returns>
        protected IActionResult BadRequestResponse(string message)
        {
            return BadRequest(new RespuestasAPI<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccess = false,
                ErrorMessages = new List<string> { message },
                Result = new { }
            });
        }

        /// <summary>
        /// Devuelve una respuesta de recurso no encontrado con estado 404 (Not Found).
        /// </summary>
        /// <param name="message">El mensaje de error que se enviará al cliente.</param>
        /// <returns>
        /// Un objeto <see cref="IActionResult"/> con el código de estado 404 y un mensaje de error.
        /// </returns>
        protected IActionResult NotFoundResponse(string message)
        {
            return NotFound(new RespuestasAPI<object>
            {
                StatusCode = HttpStatusCode.NotFound,
                IsSuccess = false,
                ErrorMessages = new List<string> { message },
                Result = new { }
            });
        }
    }
}
