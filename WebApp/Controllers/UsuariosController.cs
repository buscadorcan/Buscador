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
    /// <summary>
    /// Authenticates a user based on the provided credentials.
    /// </summary>
    /// <param name="usuarioAutenticacionDto">The user authentication data transfer object containing the username and password.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the authentication result.
    /// If the authentication is successful, returns an <see cref="OkObjectResult"/> with the user authentication response data.
    /// If the authentication fails, returns a <see cref="BadRequestObjectResult"/> with an error message.
    /// In case of an exception, returns an appropriate error response.
    /// </returns>
    [HttpPost("login")]
    public IActionResult Login([FromBody] UsuarioAutenticacionDto usuarioAutenticacionDto)
    {
      try
      {
        var result = _iService.Authenticate(usuarioAutenticacionDto);

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
    //[Authorize]
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
    //[Authorize]
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
    //[Authorize]
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

    //[Authorize]
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
