using Microsoft.AspNetCore.Authorization;
using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Models;
using AutoMapper;

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
    IMapper mapper
  ) : BaseController
  {
    private readonly IUsuarioRepository _iRepo = iRepo;
    private readonly IMapper _mapper = mapper;
    [HttpPost("login")]
    public IActionResult Login([FromBody] UsuarioAutenticacionDto usuarioAutenticacionDto)
    {
      try
      {
        var result = _iRepo.Login(usuarioAutenticacionDto);

        if (result.Usuario == null || string.IsNullOrEmpty(result.Token))
        {
          return BadRequestResponse("El nombre de usuario o password son incorrectos");
        }

        return Ok(new RespuestasAPI<UsuarioAutenticacionRespuestaDto> {
          Result = result
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
        var respuestaRecuperacion = await _iRepo.RecoverAsync(usuarioRecuperacionDto);

        if (!respuestaRecuperacion)
        {
          return BadRequestResponse("El nombre de usuario es incorrecto");
        }

        return Ok(new RespuestasAPI<bool> { });
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

    [Authorize]
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
  }
}
