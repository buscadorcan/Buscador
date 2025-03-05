using System.Configuration;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Service
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IONAConexionRepository _onaConexionRepository;
        private readonly IHashService _hashService;
        private readonly IJwtService _jwtService;
        private readonly ICatalogosRepository _catalogosRepository;
        private readonly IEventTrackingRepository _eventTrackingRepository;
        private readonly IEmailService _emailService;
        private readonly IRandomStringGeneratorService _randomGeneratorService;
        private readonly IConfiguration _configuration;
        public AuthenticateService(
            IUsuarioRepository usuarioRepository,
            IONAConexionRepository onaConexionRepository,
            ICatalogosRepository catalogosRepository,
            IEventTrackingRepository eventTrackingRepository,
            IRandomStringGeneratorService randomGeneratorService,
            IEmailService emailService,
            IHashService hashService,
            IJwtService jwtService,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _onaConexionRepository = onaConexionRepository;
            _catalogosRepository = catalogosRepository;
            _eventTrackingRepository = eventTrackingRepository;
            _randomGeneratorService = randomGeneratorService;
            _emailService = emailService;
            _hashService = hashService;
            _jwtService = jwtService;
            _configuration = configuration;
        }
        /// <inheritdoc />
        public Result<AuthenticateResponseDto> Authenticate(UsuarioAutenticacionDto usuarioAutenticacionDto)
        {
            try {
                var result = Authenticate(usuarioAutenticacionDto.Email, usuarioAutenticacionDto.Clave);

                if (!result.IsSuccess) {
                    GenerateEventTracking(dto: usuarioAutenticacionDto);
                    return Result<AuthenticateResponseDto>.Failure(result.ErrorMessage);
                }

                var usuario = result.Value;
                var rol = GetRol(usuario.IdHomologacionRol);

                string code = _randomGeneratorService.GenerateTemporaryCode(6);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var htmlBody = GenerateVerificationCodeEmailBody(code);
                        await _emailService.SendEmailAsync(usuario.Email ?? "", "Código de Verificación", htmlBody);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al enviar correo: {ex.Message}");
                    }
                });

                GenerateEventTracking(usuario: usuario, rol: rol, code: code);
                return Result<AuthenticateResponseDto>.Success(new AuthenticateResponseDto
                {
                    IdUsuario = usuario.IdUsuario,
                    IdHomologacionRol = usuario.IdHomologacionRol
                });
            } catch (Exception ex) {
                GenerateEventTracking(dto: usuarioAutenticacionDto);
                throw ex;
            }
        }
        public Result<UsuarioAutenticacionRespuestaDto> ValidateCode(AuthValidationDto authValidationDto)
        {
            try {
                if (authValidationDto.IdUsuario == 0)
                {
                    GenerateEventTracking(dto: authValidationDto);
                    return Result<UsuarioAutenticacionRespuestaDto>.Failure("Usuario Incorrecto");
                }

                var usuario = _usuarioRepository.FindById(authValidationDto.IdUsuario);
                var rol = GetRol(usuario.IdHomologacionRol);

                var code = _eventTrackingRepository.GetCodeByUser(usuario.Nombre, rol.CodigoHomologacion, "Access");
                Console.WriteLine(code);
                if (string.IsNullOrEmpty(code) || !authValidationDto.Codigo.Equals(code))
                {
                   GenerateEventTracking(dto: authValidationDto);
                    // activar despues
                   // return Result<UsuarioAutenticacionRespuestaDto>.Failure("Código Incorrecto");
                }

                var ona = _onaConexionRepository.FindById(usuario.IdONA);
                var homologacionGrupo = GetVwHomologacionGrupo();
                var token = GenerateToken(usuario.IdUsuario);

                GenerateEventTracking(dto: authValidationDto, usuario: usuario, rol: rol);
                return Result<UsuarioAutenticacionRespuestaDto>.Success(new UsuarioAutenticacionRespuestaDto
                {
                    Token = token,
                    Usuario = new UsuarioDto
                    {
                        IdUsuario = usuario.IdUsuario,
                        Email = usuario.Email,
                        Nombre = usuario.Nombre,
                        Apellido = usuario.Apellido,
                        Telefono = usuario.Telefono,
                        IdHomologacionRol = usuario.IdHomologacionRol,
                        IdONA = usuario.IdONA,
                        BaseDatos = ona.BaseDatos,
                        OrigenDatos = ona.OrigenDatos,
                        Migrar = ona.Migrar,
                        EstadoMigracion = ona.Estado
                    },
                    Rol = rol,
                    HomologacionGrupo = homologacionGrupo
                });
            }
            catch (Exception ex) {
                GenerateEventTracking(dto: authValidationDto);
                throw ex;
            }
        }

        /// <summary>
        /// Authenticates a user asynchronously using their email and password.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <param name="clave">The password of the user.</param>
        /// <returns>
        /// A <see cref="Usuario"/> object representing the authenticated user if the credentials are valid.
        /// </returns>
        /// <exception cref="Exception">
        /// Thrown under the following conditions:
        /// - If the provided email or password is null or empty.
        /// - If no user is found with the provided email.
        /// - If the provided password does not match the user's stored password.
        /// </exception>
        private Result<Usuario> Authenticate(string email, string clave)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Result<Usuario>.Failure("El correo electrónico no puede estar vacío.");
            }

            if (string.IsNullOrEmpty(clave))
            {
                return Result<Usuario>.Failure("La Clave no puede estar vacía.");
            }

            var usuario = _usuarioRepository.FindByEmail(email);

            if (usuario == null)
            {
                return Result<Usuario>.Failure("Usuario no encontrado.");
            }

            var claveHash = _hashService.GenerateHash(clave.Trim());

            if (!usuario.Clave.Equals(claveHash))
            {
                return Result<Usuario>.Failure("Clave incorrecta.");
            }

            return Result<Usuario>.Success(usuario);
        }

        /// <summary>
        /// Generates a JWT (JSON Web Token) for the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the token is generated.</param>
        /// <returns>
        /// A string representing the generated JWT token.
        /// </returns>
        private string GenerateToken(int userId)
        {
            return _jwtService.GenerateJwtToken(userId);
        }

        /// <summary>
        /// Retrieves a role data transfer object (DTO) based on the provided homologation role ID.
        /// </summary>
        /// <param name="idHomologacionRol">The unique identifier of the homologation role to search for.</param>
        /// <returns>
        /// A <see cref="VwRolDto"/> object containing the role details if found; otherwise, <c>null</c>.
        /// </returns>
        private VwRolDto? GetRol(int idHomologacionRol)
        {
            var rol = _catalogosRepository.FindVwRolByHId(idHomologacionRol);

            return rol != null 
                ? new VwRolDto 
                    {
                        IdHomologacionRol = rol.IdHomologacionRol,
                        Rol = rol.Rol,
                        CodigoHomologacion = rol.CodigoHomologacion
                    } 
                : null;
        }

        /// <summary>
        /// Retrieves a <see cref="VwHomologacionGrupoDto"/> object for the homologation group with the code "KEY_MENU".
        /// </summary>
        /// <returns>
        /// A <see cref="VwHomologacionGrupoDto"/> object containing the homologation group details if found; otherwise, <c>null</c>.
        /// </returns>
        private VwHomologacionGrupoDto? GetVwHomologacionGrupo()
        {
            var homologacionGrupo = _catalogosRepository.FindVwHGByCode("KEY_MENU");

            return homologacionGrupo != null 
                ? new VwHomologacionGrupoDto 
                    {
                        MostrarWeb = homologacionGrupo.MostrarWeb,
                        TooltipWeb = homologacionGrupo.TooltipWeb,
                        CodigoHomologacion = homologacionGrupo.CodigoHomologacion
                    } 
                : null;
        }

        private void GenerateEventTracking(
            UsuarioAutenticacionDto? dto = null,
            Usuario? usuario = null,
            VwRolDto? rol = null,
            string? code = null,
            bool success = true
        )
        {
            var eventTrackingDto = new paAddEventTrackingDto
            {
                CodigoHomologacionRol = rol?.CodigoHomologacion ?? "",
                NombreUsuario = usuario?.Nombre ?? dto.Email,
                CodigoHomologacionMenu = "Access",
                NombreControl = "btnLogin",
                NombreAccion = "acceder()",
                ParametroJson = JsonConvert.SerializeObject(usuario == null ? dto : new
                {
                    Email = usuario?.Email ?? dto.Email,
                    Success = success,
                    Code = code
                })
            };

            _eventTrackingRepository.Create(eventTrackingDto);
        }

        private void GenerateEventTracking(
            AuthValidationDto? dto = null,
            Usuario? usuario = null,
            VwRolDto? rol = null,
            bool success = true
        )
        {
            var eventTrackingDto = new paAddEventTrackingDto
            {
                CodigoHomologacionRol = rol?.CodigoHomologacion ?? "",
                NombreUsuario = usuario?.Nombre ?? $"{dto.IdUsuario}",
                CodigoHomologacionMenu = "Access",
                NombreControl = "btnValidar",
                NombreAccion = "ValidarCodigo()",
                ParametroJson = JsonConvert.SerializeObject(usuario == null ? dto : new
                {
                    Id = usuario?.IdUsuario ?? dto.IdUsuario,
                    Success = success
                })
            };

            _eventTrackingRepository.Create(eventTrackingDto);
        }

        /// <summary>
        /// Generates an HTML email body for sending a verification code to the user.
        /// </summary>
        /// <param name="codigo">The verification code to be sent in the email.</param>
        /// <returns>A string containing the HTML body of the email with the verification code inserted.</returns>
        public string GenerateVerificationCodeEmailBody(string codigo)
        {
            string templatePath = _configuration["EmailTemplates:Verification"] ?? "";

            if (File.Exists(templatePath))
            {
                string htmlBody = File.ReadAllText(templatePath);
                return string.Format(htmlBody, codigo);
            }
            else
            {
                throw new FileNotFoundException("La plantilla de correo no se encuentra en la ubicación especificada.");
            }
        }
    }
}
