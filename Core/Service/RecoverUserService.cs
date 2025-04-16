using System.Text;
using Core.Interfaces;
using DataAccess.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SharedApp.Dtos;
using DataAccess.Models;
using Core.Services;

namespace Core.Service
{
    /// <summary>
    /// Service responsible for handling user password recovery operations.
    /// </summary>
    public class RecoverUserService : IRecoverUserService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IEventTrackingRepository _eventTrackingRepository;
        private readonly ICatalogosRepository _catalogosRepository;
        private readonly IRandomStringGeneratorService _randomGeneratorService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public RecoverUserService(
            IUsuarioRepository usuarioRepository,
            IEventTrackingRepository eventTrackingRepository,
            ICatalogosRepository catalogosRepository,
            IRandomStringGeneratorService randomGeneratorService,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _usuarioRepository = usuarioRepository;
            _eventTrackingRepository = eventTrackingRepository;
            _catalogosRepository = catalogosRepository;
            _randomGeneratorService = randomGeneratorService;
            _emailService = emailService;
            _configuration = configuration;
        }

        /// <inheritdoc />
        public Result<bool> RecoverPassword(UsuarioRecuperacionDto usuarioRecuperacionDto)
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

                string clave = _randomGeneratorService.GenerateTemporaryPassword(8);
                result.Value.Clave = clave;
                var isSave = _usuarioRepository.Update(result.Value);

                if (isSave)
                {
                    var htmlBody = GenerateTemporaryKeyEmailBody(result.Value, clave);
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _emailService.SendEmailAsync(result.Value.Email ?? "", "Recuperación de Clave de Acceso al Sistema", htmlBody);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error al enviar correo: {ex.Message}");
                        }
                    });

                    return Result<bool>.Success(true);
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
                CodigoHomologacionRol = rol?.CodigoHomologacion ?? "",
                NombreUsuario = usuario?.Nombre ?? dto?.Email,
                CodigoHomologacionMenu = "RecoverPassword",
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
        /// Genera el cuerpo del correo electrónico para la recuperación de una clave temporal de acceso.
        /// </summary>
        /// <param name="usuario">El objeto <see cref="Usuario"/> que contiene los detalles del usuario (nombre y correo electrónico) que solicitaron la recuperación de clave.</param>
        /// <param name="clave">La clave temporal que se proporcionará al usuario para acceder al sistema.</param>
        /// <returns>
        /// Devuelve el cuerpo del correo electrónico como una cadena de texto en formato HTML, con la clave temporal y los datos del usuario insertados en la plantilla.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// Lanza una excepción si la plantilla de correo HTML no se encuentra en la ubicación especificada en el directorio de plantillas.
        /// </exception>
        public string GenerateTemporaryKeyEmailBody(Usuario usuario, string clave)
        {
            string templatePath = _configuration["EmailTemplates:Temporary"] ?? "";

            if (File.Exists(templatePath))
            {
                string htmlBody = File.ReadAllText(templatePath, Encoding.UTF8);
                return string.Format(htmlBody, usuario.Nombre, usuario.Email, clave);
            }
            else
            {
                throw new FileNotFoundException("La plantilla de correo no se encuentra en la ubicación especificada.");
            }
        }
    }
}
