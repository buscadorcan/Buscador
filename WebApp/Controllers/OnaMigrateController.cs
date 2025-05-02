using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Dtos;
using SharedApp.Response;

namespace WebApp.Controllers
{
    [Route(Routes.ONAC)]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public class OnaMigrateController : BaseController
    {
        private readonly IOnaMigrate _ionaMigrate;

        public OnaMigrateController(IOnaMigrate IonaMigrate, 
             ILogger<BaseController> logger) : base(logger)
        {
            _ionaMigrate = IonaMigrate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(Routes.POST_ONA_MIGRATE)]
        public async Task<IActionResult> postOnaMigrate([FromBody] OnaMigrateRequestDto request)
        {
            try
            {
                return Ok(new RespuestasAPI<List<OnaMigrateDto>>
                {
                    Result = (await _ionaMigrate.postOnaMigrate(request.vista, request.IdOna, request.IdEsquema)).ToList()
                });

            }
            catch (Exception e)
            {
                return HandleException(e, nameof(postOnaMigrate));
            }
        }

    }
}
