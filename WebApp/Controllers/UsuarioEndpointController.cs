using WebApp.Repositories.IRepositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models;
using SharedApp.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using WebApp.Models;

namespace WebApp.Controllers
{
  [Route("api/permiso")]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public class UsuarioEndpointController(
    ILogger<UsuarioEndpointController> logger,
    IUsuarioEndpointRepository iRepo,
    IMapper mapper
  ) : BaseController
  {
    private readonly IUsuarioEndpointRepository _iRepo = iRepo;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UsuarioEndpointController> _logger = logger;

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/Create: Crea un nuevo registro de usuario con permisos de endpoint.
     */
    [Authorize]
    [HttpPost]
    public IActionResult Create([FromBody] UsuarioEndpointPermisoRegistroDto data)
    {
      try
      {
        return Ok(new RespuestasAPI<bool> {
          IsSuccess = _iRepo.Create(_mapper.Map<UsuarioEndpoint>(data))
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Create));
      }
    }

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/FindAll: Obtiene todos los registros de usuarios con sus permisos de endpoint.
     */
    [Authorize]
    [HttpGet]
    public IActionResult FindAll()
    {
      try
      {
        return Ok(new RespuestasAPI<List<UsuarioEndpointPermisoDto>>{
          Result = _mapper.Map<List<UsuarioEndpointPermisoDto>>(_iRepo.FindAll())
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(FindAll));
      }
    }

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/FindByEndpointId: Obtiene información del usuario asociado a un endpoint específico.
     */
    [Authorize]
    [HttpGet("{endpointId:int}", Name = "FindByEndpointId")]
    public IActionResult FindByEndpointId(int endpointId)
    {
      try
      {
        var itemUsuario = _iRepo.FindByEndpointId(endpointId);

        if (itemUsuario == null)
        {
          return NotFoundResponse("Registro no encontrado");
        }

        return Ok(new RespuestasAPI<UsuarioDto> {
          Result = _mapper.Map<UsuarioDto>(itemUsuario)
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(FindByEndpointId));
      }
    }
  }
}
