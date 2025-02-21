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
         /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/HandleException: Maneja excepciones no controladas y devuelve una respuesta de error con estado 500 (Internal Server Error).
         */
        protected IActionResult HandleException(Exception e, string methodName)
        {
          try
          {
              var logger = (ILogger<BaseController>?)HttpContext.RequestServices.GetService(typeof(ILogger<BaseController>));
              if (logger != null)
              {
                  logger.LogError(e, $"Error en {methodName}");
              }
          }
          catch (Exception) {}
           
          return StatusCode(StatusCodes.Status500InternalServerError, new RespuestasAPI<object>
          {
            StatusCode = HttpStatusCode.InternalServerError,
            IsSuccess = false,
            ErrorMessages = new List<string> { "Error en el servidor" },
            Result = new { }
          });
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/BadRequestResponse: Devuelve una respuesta de solicitud incorrecta con estado 400 (Bad Request).
         */
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

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/BadRequestResponse: Devuelve una respuesta de solicitud incorrecta con estado 400 (Bad Request).
         */
        protected IActionResult BadRequestResponse(string message, object result)
        {
            return BadRequest(new RespuestasAPI<object>
            {
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccess = false,
                ErrorMessages = new List<string> { message },
                Result = result
            });
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
         * WebApp/NotFoundResponse: Devuelve una respuesta de recurso no encontrado con estado 404 (Not Found).
         */
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
