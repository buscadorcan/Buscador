using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using WebApp.Service.IService;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class EmailController(IEmailService emailService) : BaseController
    {
        private readonly IEmailService _emailService = emailService;

        [HttpPost("enviar")]
        public async Task<IActionResult> EnviarCorreoRol([FromBody] EmailDto email) {
            try
            {
                var result = await _emailService.SendEmailAsync("", "", "");
                return Ok(new RespuestasAPI<bool>() { Result = result });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(EnviarCorreoRol));
            }
        }

        
    }
}
