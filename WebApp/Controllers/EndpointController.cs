using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using WebApp.Repositories.IRepositories;

namespace WebApp.Controllers
{
    [Route("api/endpoint")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class EndpointController(
        IEndpointRepository usRepo,
        IMapper mapper
    ) : BaseController
    {
        private readonly IEndpointRepository _usRepo = usRepo;
        private readonly IMapper _mapper = mapper;
        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] EndpointDto endpointDto)
        {
            try
            {
                bool isUnique = _usRepo.IsUniqueUserUrl(endpointDto.Nombre, endpointDto.Url);
                if (!isUnique)
                {
                    return BadRequestResponse("El endpoint ya existe");
                }

                return Ok(new RespuestasAPI {
                    IsSuccess = _usRepo.Create(_mapper.Map<Models.Endpoint>(endpointDto))
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Create));
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult FindAll()
        {
            try
            {
                var listaEndpoint = _usRepo.FindAll();

                return Ok(new RespuestasAPI {
                    Result = _mapper.Map<List<EndpointDto>>(listaEndpoint)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindAll));
            }
        }
        [Authorize]
        [HttpGet("{endpointId:int}", Name = "GetEndpoint")]
        public IActionResult FindById(int endpointId)
        {
            try
            {
                var itemEndpoint = _usRepo.FindById(endpointId);

                if (itemEndpoint == null)
                {
                    return NotFoundResponse("Endpoint no encontrado");
                }

                return Ok(new RespuestasAPI {
                    Result = _mapper.Map<EndpointDto>(itemEndpoint)
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(FindById));
            }
        }
    }
}