using Microsoft.AspNetCore.Authorization;
using WebApp.Repositories.IRepositories;
using Microsoft.AspNetCore.Mvc;
using SharedApp.Models.Dtos;
using SharedApp.Models;
using WebApp.Models;
using AutoMapper;

namespace WebApp.Controllers
{
  /// <summary>
  /// Controlador para gestionar las operaciones relacionadas con usuarios, incluyendo autenticación,
  /// recuperación de contraseñas, creación, actualización, eliminación y consulta de usuarios.
  /// </summary>
  [Route("api")]
  [ApiController]
  public class UsuariosController(
    IUsuarioRepository iRepo,
    IMapper mapper
  ) : BaseController
  {
    private readonly IUsuarioRepository _iRepo = iRepo;
    private readonly IMapper _mapper = mapper;

    /// <summary>
    /// Autentica a un usuario y genera un token si las credenciales son válidas.
    /// </summary>
    /// <param name="usuarioAutenticacionDto">DTO con email y contraseña del usuario.</param>
    /// <returns>El usuario autenticado junto con su token.</returns>
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

        return Ok(new RespuestasAPI<UsuarioAutenticacionRespuestaDto> { Result = result });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Login));
      }
    }

    /// <summary>
    /// Envía instrucciones para recuperar la contraseña de un usuario.
    /// </summary>
    /// <param name="usuarioRecuperacionDto">DTO con el email del usuario.</param>
    /// <returns>Una respuesta indicando si el proceso fue exitoso.</returns>
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

        return Ok(new RespuestasAPI<bool>());
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(RecoverAsync));
      }
    }

    /// <summary>
    /// Registra un nuevo usuario.
    /// </summary>
    /// <param name="dto">DTO con los datos del usuario a registrar.</param>
    /// <returns>Una respuesta indicando si el registro fue exitoso.</returns>
    [Authorize]
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

        return Ok(new RespuestasAPI<bool>
        {
          IsSuccess = _iRepo.Create(_mapper.Map<Usuario>(dto))
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Create));
      }
    }

    /// <summary>
    /// Obtiene una lista de todos los usuarios.
    /// </summary>
    /// <returns>Lista de usuarios registrados.</returns>
    [Authorize]
    [HttpGet]
    public IActionResult FindAll()
    {
      try
      {
        return Ok(new RespuestasAPI<List<UsuarioDto>>
        {
          Result = _mapper.Map<List<UsuarioDto>>(_iRepo.FindAll())
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(FindAll));
      }
    }

    /// <summary>
    /// Obtiene un usuario por su ID.
    /// </summary>
    /// <param name="idUsuario">ID del usuario a buscar.</param>
    /// <returns>Los datos del usuario encontrado.</returns>
    [Authorize]
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

        return Ok(new RespuestasAPI<UsuarioDto>
        {
          Result = _mapper.Map<UsuarioDto>(itemUsuario)
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(FindById));
      }
    }

    /// <summary>
    /// Actualiza la información de un usuario.
    /// </summary>
    /// <param name="idUsuario">ID del usuario a actualizar.</param>
    /// <param name="dto">DTO con los datos actualizados.</param>
    /// <returns>Una respuesta indicando si la actualización fue exitosa.</returns>
    [Authorize]
    [HttpPut("{idUsuario:int}", Name = "Update")]
    public IActionResult Update(int idUsuario, [FromBody] UsuarioDto dto)
    {
      try
      {
        dto.IdUsuario = idUsuario;
        var usuario = _mapper.Map<Usuario>(dto);

        return Ok(new RespuestasAPI<bool>
        {
          IsSuccess = _iRepo.Update(usuario)
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Update));
      }
    }

    /// <summary>
    /// Desactiva un usuario cambiando su estado.
    /// </summary>
    /// <param name="idUsuario">ID del usuario a desactivar.</param>
    /// <returns>Una respuesta indicando si la desactivación fue exitosa.</returns>
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

        return Ok(new RespuestasAPI<bool>
        {
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
