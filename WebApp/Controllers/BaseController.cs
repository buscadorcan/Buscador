using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using System.Net;

namespace WebApp.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult HandleException(Exception e, string methodName)
        {
            var logger = (ILogger<BaseController>?)HttpContext.RequestServices.GetService(typeof(ILogger<BaseController>));
            if (logger != null)
            {
                logger.LogError(e, $"Error en {methodName}");
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new RespuestasAPI
            {
                StatusCode = HttpStatusCode.InternalServerError,
                IsSuccess = false,
                ErrorMessages = new List<string> { "Error en el servidor" }
            });
        }

        protected IActionResult BadRequestResponse(string message)
        {
            return BadRequest(new RespuestasAPI
            {
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccess = false,
                ErrorMessages = new List<string> { message }
            });
        }
        protected IActionResult NotFoundResponse(string message)
        {
            return NotFound(new RespuestasAPI
            {
                StatusCode = HttpStatusCode.NotFound,
                IsSuccess = false,
                ErrorMessages = new List<string> { message }
            });
        }
    }
}