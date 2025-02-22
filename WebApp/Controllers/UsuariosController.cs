/// Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
/// WebApp/UsuariosController: Controlador para funcionalidad en Usuarios
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
        /// WebApp/Login: Autentica a un usuario en el sistema y devuelve sus credenciales.
        /// Este método permite autenticar a un usuario en el sistema mediante sus credenciales de acceso.
        /// </summary>
        /// <param name="usuarioAutenticacionDto">Objeto que contiene el correo electrónico y la contraseña del usuario.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con la respuesta de autenticación que incluye el token de acceso si es exitoso.
        /// En caso de credenciales inválidas, devuelve un mensaje de error.
        /// </returns>
        [HttpPost("login")]
    public IActionResult Login([FromBody] UsuarioAutenticacionDto usuarioAutenticacionDto)
        {
            try
            {
        var result = _iService.Authenticate(usuarioAutenticacionDto);

                if (!result.IsSuccess)
                {
                    return BadRequestResponse(result.ErrorMessage);
                }

                return Ok(new RespuestasAPI<AuthenticateResponseDto>
                {
                    Result = result.Value
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Login));
            }
        }

        /// <summary>
        /// WebApp/validar: Valida el código enviado luego de la autenticación.
        /// Este método permite validar el código de autenticación de múltiples factores (MFA) enviado al usuario.
        /// </summary>
        /// <param name="authValidationDto">Objeto que contiene el código de validación y la información del usuario.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con la información de autenticación del usuario si la validación es exitosa.
        /// En caso de código inválido, devuelve un mensaje de error.
        /// </returns>
        [HttpPost("validar")]
        public IActionResult Validar([FromBody] AuthValidationDto authValidationDto)
        {
            try
            {
                var result = _iService.ValidateCode(authValidationDto);

                if (!result.IsSuccess)
                {
                    return BadRequestResponse(result.ErrorMessage);
                }

                return Ok(new RespuestasAPI<UsuarioAutenticacionRespuestaDto>
                {
                    Result = result.Value
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(Validar));
            }
        }

        /// <summary>
        /// WebApp/RecoverAsync: Permite la recuperación de contraseña de un usuario.
        /// Este método envía un enlace o código de recuperación de contraseña al correo del usuario para permitir el restablecimiento.
        /// </summary>
        /// <param name = "usuarioRecuperacionDto" > Objeto que contiene el correo electrónico del usuario que desea recuperar la contraseña.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la solicitud de recuperación fue exitosa.
        /// </returns>
        [HttpPost("recuperar")]
        public IActionResult RecoverAsync([FromBody] UsuarioRecuperacionDto usuarioRecuperacionDto)
        {
            try
            {
                var result = _iServiceRecover.RecoverPassword(usuarioRecuperacionDto);

                if (!result.IsSuccess)
                {
                    return BadRequestResponse(result.ErrorMessage);
                }

                return Ok(new RespuestasAPI<bool>
                {
                    Result = result.Value
                });
            }
            catch (Exception e)
            {
                return HandleException(e, nameof(RecoverAsync));
            }
        }

        /// <summary>
        /// WebApp/Create: Registra un nuevo usuario en el sistema.
        /// Este método permite crear una nueva cuenta de usuario con sus datos de registro.
        /// </summary>
        /// <param name="dto">Objeto que contiene la información del nuevo usuario, incluyendo su correo y contraseña.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la creación del usuario fue exitosa.
        /// Si el correo ya está registrado, devuelve un mensaje de error.
        /// </returns>
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
        /// WebApp/FindAll: Obtiene la lista de todos los usuarios registrados en el sistema.
        /// Este método permite recuperar la lista completa de usuarios con su información básica.
        /// </summary>
        /// <returns>
        /// Devuelve un objeto IActionResult con la lista de usuarios registrados en el sistema.
        /// </returns>
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
        /// FindById
        /// </summary>
        /// <param name="idUsuario">ID único del usuario a buscar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con la información del usuario solicitado.
        /// Si el usuario no existe, devuelve un mensaje de error.
        /// </returns>
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
        /// Update
        /// </summary>
        /// <param name="idUsuario">ID único del usuario a actualizar.</param>
        /// <param name="dto">Objeto con los nuevos datos del usuario.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la actualización fue exitosa.
        /// </returns>
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
        /// Deactivate
        /// </summary>
        /// <param name="idUsuario">ID único del usuario a desactivar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult indicando si la desactivación fue exitosa.
        /// Si el usuario no existe, devuelve un mensaje de error.
        /// </returns>
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

        /// <summary>
        /// ValidarEmail
        /// </summary>
        /// <param name="email">Correo electrónico a validar.</param>
        /// <returns>
        /// Devuelve un objeto IActionResult con un valor booleano indicando si el correo es único en el sistema.
        /// </returns>
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

    /* 
     * Copyright © SIDESOFT | BuscadorAndino | 2025.Feb.18
     * WebApp/ValidarEmail: Cambia las claves de un usuario en especifico.
    */
    [Authorize]
    [HttpPost("cambiar_clave")]
    public IActionResult CambiarClave([FromBody] UsuarioCambiarClaveDto usuario)
    {
      try
      {
        var result = _iRepo.ChangePasswd(usuario.Clave, usuario.ClaveNueva);

        if (!result.IsSuccess) {
          return BadRequestResponse(result.ErrorMessage);
        }

        return Ok(new RespuestasAPI<bool> {
          Result = result.Value
        });
      }
      catch (Exception e)
      {
        return HandleException(e, nameof(Login));
      }
        }

    }
}
