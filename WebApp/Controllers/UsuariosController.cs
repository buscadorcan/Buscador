using Microsoft.AspNetCore.Authorization;
using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Models;
using AutoMapper;
using WebApp.Service.IService;
using System.Net;

namespace WebApp.Controllers
{
  [Route("api/usuarios")]
  [ApiController]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
  [ProducesResponseType(StatusCodes.Status401Unauthorized)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
  public class UsuariosController(
    IUsuarioRepository iRepo,
    IMapper mapper,
    IAuthenticateService iService,
    IRecoverUserService iServiceRecover
  ) : BaseController
  {
    private readonly IUsuarioRepository _iRepo = iRepo;
    private readonly IAuthenticateService _iService = iService;
    private readonly IRecoverUserService _iServiceRecover = iServiceRecover;
    private readonly IMapper _mapper = mapper;

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/Login: Autentica a un usuario en el sistema y devuelve sus credenciales.
     */
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioAutenticacionDto usuarioAutenticacionDto)
    {
      try
      {
        var result = await _iService.Authenticate(usuarioAutenticacionDto);

        if (!result.IsSuccess) {
          return BadRequestResponse(result.ErrorMessage);
        }

        return Ok(new RespuestasAPI<AuthenticateResponseDto> {
          Result = result.Value
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Login));
      }
    }

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/validar: Valida el codigo enviado luego de la autenticación.
     */
    [HttpPost("validar")]
    public IActionResult Validar([FromBody] AuthValidationDto authValidationDto)
    {
      try
      {
        var result = _iService.ValidateCode(authValidationDto);

        if (!result.IsSuccess) {
          return BadRequestResponse(result.ErrorMessage);
        }

        return Ok(new RespuestasAPI<UsuarioAutenticacionRespuestaDto> {
          Result = result.Value
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Login));
      }
    }

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/RecoverAsync: Permite la recuperación de contraseña de un usuario.
     */
    [HttpPost("recuperar")]
    public async Task<IActionResult> RecoverAsync([FromBody] UsuarioRecuperacionDto usuarioRecuperacionDto)
    {
      try
      {
        var result = await _iServiceRecover.RecoverPassword(usuarioRecuperacionDto);

        if (!result.IsSuccess) {
          return BadRequestResponse(result.ErrorMessage);
        }

        return Ok(new RespuestasAPI<bool> {
          Result = result.Value
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(RecoverAsync));
      }
    }

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/Create: Registra un nuevo usuario en el sistema.
     */
    [HttpPost("registro")]
    public IActionResult Create([FromBody] UsuarioDto dto)
    {
      try
      {
        bool validarEmailUnico = _iRepo.IsUniqueUser(dto.Email ?? "");
        if (!validarEmailUnico)
        {
          return BadRequestResponse("El nombre de usuario ya existe");
        }

        return Ok(new RespuestasAPI<bool> {
          IsSuccess = _iRepo.Create(_mapper.Map<Usuario>(dto))
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Create));
      }
    }

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/FindAll: Obtiene la lista de todos los usuarios registrados en el sistema.
     */
    [HttpGet]
    public IActionResult FindAll()
    {
      try
      {
        return Ok(new RespuestasAPI<List<UsuarioDto>> {
          Result = _mapper.Map<List<UsuarioDto>>(_iRepo.FindAll())
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(FindAll));
      }
    }


    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/FindById: Obtiene la información de un usuario específico según su ID.
     */
    [HttpGet("{idUsuario:int}", Name = "FindById")]
    public IActionResult FindById(int idUsuario)
    {
      try
      {
        var itemUsuario = _iRepo.FindById(idUsuario);

        if (itemUsuario == null)
        {
          return NotFoundResponse("Usuario no encontrado");
        }

        return Ok(new RespuestasAPI<UsuarioDto> {
          Result = _mapper.Map<UsuarioDto>(itemUsuario)
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(FindById));
      }
    }


    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/Update: Actualiza la información de un usuario en el sistema.
     */
    [HttpPut("{idUsuario:int}", Name = "Update")]
    public IActionResult Update(int idUsuario, [FromBody] UsuarioDto dto)
    {
      try
      {
        dto.IdUsuario = idUsuario;
        var usuario = _mapper.Map<Usuario>(dto);

        return Ok(new RespuestasAPI<bool> {
          IsSuccess = _iRepo.Update(usuario)
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Update));
      }
    }

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/Deactivate: Desactiva un usuario estableciendo su estado como "X".
     */
    [Authorize]
    [HttpDelete("{idUsuario:int}", Name = "Deactivate")]
    public IActionResult Deactivate(int idUsuario)
    {
      try
      {
        var usuario = _iRepo.FindById(idUsuario);

        if (usuario == null)
        {
          return NotFoundResponse("Id de Usuario incorrecto");
        }

        usuario.Estado = "X";

        return Ok(new RespuestasAPI<bool> {
          IsSuccess = _iRepo.Update(usuario)
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Deactivate));
      }
    }

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/ValidarEmail: Verifica si un correo electrónico ya está registrado en el sistema.
     */
    [HttpGet("validar-email")]
    public IActionResult ValidarEmail([FromQuery] string email)
    {
        try
        {
            bool isUnique = _iRepo.IsUniqueUser(email);
            return Ok(isUnique);
        }
        catch (Exception e)
        {
            return HandleException(e, nameof(ValidarEmail));
        }
        
    }
  }
}
