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
        public AuthenticateService(
            IUsuarioRepository usuarioRepository,
            IONAConexionRepository onaConexionRepository,
            ICatalogosRepository catalogosRepository,
            IEventTrackingRepository eventTrackingRepository,
            IRandomStringGeneratorService randomGeneratorService,
            IEmailService emailService,
            IHashService hashService,
            IJwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _onaConexionRepository = onaConexionRepository;
            _catalogosRepository = catalogosRepository;
            _eventTrackingRepository = eventTrackingRepository;
            _randomGeneratorService = randomGeneratorService;
            _emailService = emailService;
            _hashService = hashService;
            _jwtService = jwtService;
        }
        /// <inheritdoc />
        public async Task<Result<AuthenticateResponseDto>> Authenticate(UsuarioAutenticacionDto usuarioAutenticacionDto)
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
                var htmlBody = GenerateVerificationCodeEmailBody(code);
                //var isSend = await _emailService.EnviarCorreoAsync(usuario.Email ?? "", "Código de Verificación", htmlBody);

                //if (!isSend)
                //{
                //    return Result<AuthenticateResponseDto>.Failure("Error al enviar clave temporal");
                //}

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
                    return Result<UsuarioAutenticacionRespuestaDto>.Failure("Código Incorrecto");
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
                TipoUsuario = rol?.CodigoHomologacion ?? "",
                NombreUsuario = usuario?.Nombre ?? dto.Email,
                NombrePagina = "Access",
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
                TipoUsuario = rol?.CodigoHomologacion ?? "",
                NombreUsuario = usuario?.Nombre ?? $"{dto.IdUsuario}",
                NombrePagina = "Access",
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
            string htmlBody = @"
            <!DOCTYPE html>
            <html lang='es'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Código de Verificación</title>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f9f9f9;
                        color: #333;
                        padding: 20px;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: 0 auto;
                        background-color: #fff;
                        padding: 30px;
                        border-radius: 8px;
                        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                    }}
                    h2 {{
                        color: #007bff;
                        text-align: center;
                    }}
                    p {{
                        font-size: 16px;
                        line-height: 1.5;
                    }}
                    .code {{
                        display: inline-block;
                        background-color: #f8f9fa;
                        border: 1px solid #ddd;
                        padding: 10px;
                        font-size: 18px;
                        font-weight: bold;
                        color: #007bff;
                        border-radius: 5px;
                    }}
                    .footer {{
                        font-size: 14px;
                        text-align: center;
                        margin-top: 20px;
                        color: #888;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <h2>Código de Verificación</h2>
                    <p>Estimado/a <strong>usuario</strong>,</p>
                    <p>Hemos recibido una solicitud para verificar su cuenta. A continuación, le proporcionamos su código de verificación:</p>
                    
                    <p><span class='code'>{0}</span></p>
                    
                    <p>Este código es válido por un tiempo limitado. Por favor, ingréselo en la página de verificación.</p>
                    
                    <p>Si no ha solicitado este código de verificación, por favor contacte a nuestro soporte inmediatamente.</p>
                    
                    <div class='footer'>
                        <p>Gracias por confiar en nosotros. Si tiene alguna duda, no dude en comunicarse con nuestro equipo de soporte.</p>
                        <p>&copy; 2025 Su Empresa | Todos los derechos reservados</p>
                    </div>
                </div>
            </body>
            </html>";

            return string.Format(htmlBody, codigo);
        }
    }
}
