using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using WebApp.Service;
using WebApp.Service.IService;

namespace WebApp.Controllers
{
    [Route("api/onac")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public class OnaMigrateController(IOnaMigrate IonaMigrate) : BaseController
    {
        private readonly IOnaMigrate _IonaMigrate = IonaMigrate;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("postOnaMigrate")]
        public async Task<IActionResult> postOnaMigrate([FromBody] OnaMigrateRequestDto request)
        {
            try
            {
                return Ok(new RespuestasAPI<List<OnaMigrateDto>>
                {
                    Result = (await _IonaMigrate.postOnaMigrate(request.vista, request.IdOna, request.IdEsquema)).ToList()
                });

            }
            catch (Exception e)
            {
                return HandleException(e, nameof(postOnaMigrate));
            }
        }

    }
}
