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
        public AuthenticateService(
            IUsuarioRepository usuarioRepository,
            IONAConexionRepository onaConexionRepository,
            ICatalogosRepository catalogosRepository,
            IEventTrackingRepository eventTrackingRepository,
            IHashService hashService,
            IJwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _onaConexionRepository = onaConexionRepository;
            _catalogosRepository = catalogosRepository;
            _eventTrackingRepository = eventTrackingRepository;
            _hashService = hashService;
            _jwtService = jwtService;
        }
        /// <inheritdoc />
        public Result<UsuarioAutenticacionRespuestaDto> Authenticate(UsuarioAutenticacionDto usuarioAutenticacionDto)
        {
            try {
                var result = Authenticate(usuarioAutenticacionDto.Email, usuarioAutenticacionDto.Clave);

                if (!result.IsSuccess) {
                    GenerateEventTracking(dto: usuarioAutenticacionDto);
                    return Result<UsuarioAutenticacionRespuestaDto>.Failure(result.ErrorMessage);
                }

                var usuario = result.Value;
                var ona = _onaConexionRepository.FindById(usuario.IdONA);
                var rol = GetRol(usuario.IdHomologacionRol);
                var homologacionGrupo = GetVwHomologacionGrupo();
                var token = GenerateToken(usuario.IdUsuario);

                GenerateEventTracking(usuario: usuario, rol: rol);
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
            } catch (Exception ex) {
                GenerateEventTracking(dto: usuarioAutenticacionDto);
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

            Console.WriteLine($"{usuario.Clave} / {claveHash} / {clave}");

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

        /// <summary>
        /// Generates an event tracking entry when a user attempts to log in.
        /// </summary>
        /// <param name="dto">
        /// An instance of <see cref="UsuarioAutenticacionDto"/> containing user authentication details.
        /// Used when logging in with authentication DTO instead of a user entity.
        /// </param>
        /// <param name="usuario">
        /// An instance of <see cref="Usuario"/> representing the user entity.
        /// Used when logging in with a user object instead of an authentication DTO.
        /// </param>
        /// <param name="rol">
        /// An instance of <see cref="VwRolDto"/> representing the user's role.
        /// Used to define the user type in event tracking.
        /// </param>
        /// <param name="success">
        /// A boolean flag indicating whether the login attempt was successful. Defaults to <c>true</c>.
        /// </param>
        private void GenerateEventTracking(UsuarioAutenticacionDto? dto = null, Usuario? usuario = null, VwRolDto? rol = null, bool success = true)
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
                    Success = success
                })
            };

            _eventTrackingRepository.Create(eventTrackingDto);
        }
    }
}
