/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/BaseController: Controlador para Base
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using System.Net;

namespace WebApp.Controllers
{
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// HandleException
        /// </summary>
        /// <param name="e"></param>
        /// <param name="methodName"></param>
        /// <returns>StatusCode</returns>
        /// <returns>IsSuccess</returns>
        /// <returns>ErrorMessages</returns>
        /// <returns>Result</returns>
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
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccess = false,
                ErrorMessages = new List<string> { message },
                Result = result
            });
        }

        /* 
         * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
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
