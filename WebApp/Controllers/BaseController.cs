/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/BaseController: Controlador para Base
using Microsoft.AspNetCore.Mvc;
using SharedApp.Response;
using System.Net;
using System.Runtime.CompilerServices;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controlador base para la aplicación.
    /// </summary>
    public class BaseController : ControllerBase
    {
        private readonly ILogger<BaseController> _logger;

        /// <summary>
        ///  inicia el constructor con para poder iniciar los log
        /// </summary>
        /// <param name="logger"></param>
        protected BaseController(ILogger<BaseController> logger)
        {
            this._logger = logger;
        }
        
        /// <summary>
        /// HandleException
        /// </summary>
        /// <param name="e"></param>
        /// <param name="methodName"></param>
        /// <returns>StatusCode</returns>
        /// <returns>IsSuccess</returns>
        /// <returns>ErrorMessages</returns>
        /// <returns>Result</returns>
        protected IActionResult HandleException(Exception e, 
            string methodName, 
            [CallerMemberName] string? caller = null)
        {
            _logger.LogError(e,
            "Error en {Method}::{Caller} — RequestId {RequestId}",
            methodName,
            caller,
            HttpContext.TraceIdentifier);

            // Mensaje breve al cliente
            var respuesta = new RespuestasAPI<object>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                IsSuccess = false,
                ErrorMessages = new List<string> { "Error interno del servidor" },
                Result = new { }
            };

            return StatusCode(StatusCodes.Status500InternalServerError, respuesta);
        }

        /// <summary>
        /// BadRequestResponse
        /// </summary>
        /// <param name="message">Mensaje de error a incluir en la respuesta.</param>
        /// <returns>StatusCode</returns>
        /// <returns>IsSuccess</returns>
        /// <returns>ErrorMessages</returns>
        /// <returns>Result</returns>
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
        /// NotFoundResponse
        /// </summary>
        /// <param name="message">Mensaje de error a incluir en la respuesta.</param>
        /// <returns>StatusCode</returns>
        /// <returns>IsSuccess</returns>
        /// <returns>ErrorMessages</returns>
        /// <returns>Result</returns>
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
