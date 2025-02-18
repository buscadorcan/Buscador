using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using WebApp.Models;
using WebApp.Repositories.IRepositories;
using WebApp.Service.IService;

namespace WebApp.Service
{
    /// <summary>
    /// Service responsible for handling user password recovery operations.
    /// </summary>
    public class RecoverUserService : IRecoverUserService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEventTrackingRepository _eventTrackingRepository;
        private readonly ICatalogosRepository _catalogosRepository;
        private readonly IPasswordService _passwordService;
        private readonly IEmailService _emailService;

        public RecoverUserService(
            IUsuarioRepository usuarioRepository,
            IEventTrackingRepository eventTrackingRepository,
            ICatalogosRepository catalogosRepository,
            IPasswordService passwordService,
            IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _eventTrackingRepository = eventTrackingRepository;
            _catalogosRepository = catalogosRepository;
            _passwordService = passwordService;
            _emailService = emailService;
        }

        /// <inheritdoc />
        public async Task<Result<bool>> RecoverPassword(UsuarioRecuperacionDto usuarioRecuperacionDto)
        {
            try
            {
                var result = GetUser(usuarioRecuperacionDto.Email);

                if (!result.IsSuccess)
                {
                    GenerateEventTracking(dto: usuarioRecuperacionDto);
                    return Result<bool>.Failure(result.ErrorMessage);
                }

                var rol = _catalogosRepository.FindVwRolByHId(result.Value.IdHomologacionRol);
                GenerateEventTracking(usuario: result.Value, rol: rol);

                string clave = _passwordService.GenerateTemporaryPassword(8);
                result.Value.Clave = clave;
                var isSave = _usuarioRepository.Update(result.Value);

                if (isSave)
                {
                    var htmlBody = GenerateTemporaryKeyEmailBody(clave);
                    var isSend = await _emailService.EnviarCorreoAsync(result.Value.Email ?? "", "Nueva Clave Temporal", htmlBody);

                    if (isSend)
                    {
                        return Result<bool>.Success(true);
                    }

                    return Result<bool>.Failure("Error al enviar clave temporal");
                }

                return Result<bool>.Failure("Error al generar clave temporal");
            }
            catch (Exception ex)
            {
                GenerateEventTracking(dto: usuarioRecuperacionDto);
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <returns>
        /// A <see cref="Result{T}"/> object containing the user if found; otherwise, an error message.
        /// </returns>
        private Result<Usuario> GetUser(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Result<Usuario>.Failure("El correo electrónico no puede estar vacío.");
            }

            var usuario = _usuarioRepository.FindByEmail(email);

            if (usuario == null)
            {
                return Result<Usuario>.Failure("Usuario no encontrado.");
            }

            return Result<Usuario>.Success(usuario);
        }

        /// <summary>
        /// Generates an event tracking record for password recovery operations.
        /// </summary>
        /// <param name="dto">The data transfer object containing recovery information (optional).</param>
        /// <param name="usuario">The user object (optional).</param>
        /// <param name="rol">The role object (optional).</param>
        /// <param name="success">Indicates whether the operation was successful (default: true).</param>
        private void GenerateEventTracking(UsuarioRecuperacionDto? dto = null, Usuario? usuario = null, VwRol? rol = null, bool success = true)
        {
            var eventTrackingDto = new paAddEventTrackingDto
            {
                TipoUsuario = rol?.CodigoHomologacion ?? "",
                NombreUsuario = usuario?.Nombre ?? dto?.Email,
                NombrePagina = "RecoverPassword",
                NombreControl = "btnRecover",
                NombreAccion = "recuperar()",
                ParametroJson = JsonConvert.SerializeObject(usuario == null ? dto : new
                {
                    Email = usuario?.Email ?? dto?.Email,
                    Success = success
                })
            };

            _eventTrackingRepository.Create(eventTrackingDto);
        }

        /// <summary>
        /// Generates the HTML body for the temporary password email.
        /// </summary>
        /// <param name="clave">The temporary password to include in the email.</param>
        /// <returns>
        /// A string containing the HTML body of the email.
        /// </returns>
        public string GenerateTemporaryKeyEmailBody(string clave)
        {
            string htmlBody = @"
            <!DOCTYPE html>
            <html lang='es'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Clave Temporal</title>
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
                    <h2>Su Nueva Clave Temporal</h2>
                    <p>Estimado/a <strong>usuario</strong>,</p>
                    <p>Hemos recibido una solicitud para restablecer su clave. A continuación, le proporcionamos su nueva clave temporal:</p>
                    
                    <p><span class='code'>{0}</span></p>
                    
                    <p>Recuerde que esta clave es válida por un tiempo limitado. Por su seguridad, le recomendamos cambiarla lo antes posible.</p>
                    
                    <p>Si no ha solicitado un cambio de clave, por favor contacte a nuestro soporte inmediatamente.</p>
                    
                    <div class='footer'>
                        <p>Gracias por confiar en nosotros. Si tiene alguna duda, no dude en comunicarse con nuestro equipo de soporte.</p>
                        <p>&copy; 2025 Su Empresa | Todos los derechos reservados</p>
                    </div>
                </div>
            </body>
            </html>";

            return string.Format(htmlBody, clave);
        }
    }
}
