using WebApp.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using WebApp.Service.IService;
using SharedApp.Models.Dtos;
using WebApp.Models;
using Newtonsoft.Json;

namespace WebApp.Repositories
{
    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        private readonly IJwtService _jwtService;
        private readonly IHashService _hashService;
        private readonly IEmailService _emailService;
        private readonly IEventTrackingRepository _eventTrackingRepository;
        public UsuarioRepository (
            IJwtService jwtService,
            IEmailService emailService,
            IHashService hashService,
            ILogger<UsuarioRepository> logger,
            ISqlServerDbContextFactory sqlServerDbContextFactory,
            IEventTrackingRepository eventTrackingRepository
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
            _hashService = hashService;
            _emailService = emailService;
            _eventTrackingRepository = eventTrackingRepository;
        }
        public Usuario? FindById(int idUsuario)
        {
            // return ExecuteDbOperation(context => context.Usuario.AsNoTracking().FirstOrDefault(c => c.IdUsuario == idUsuario));
            return ExecuteDbOperation(context =>
            {
                // Realizamos el join entre Usuario, Homologacion y ONA
                var query = from usuario in context.Usuario.AsNoTracking()
                            join homologacion in context.VwRol.AsNoTracking()
                            on usuario.IdHomologacionRol equals homologacion.IdHomologacionRol into homologacionJoin
                            from homologacion in homologacionJoin.DefaultIfEmpty()
                            join ona in context.ONA.AsNoTracking()
                            on usuario.IdONA equals ona.IdONA
                            where usuario.IdUsuario == idUsuario // Filtramos por idUsuario
                            orderby usuario.IdUsuario
                            select new Usuario
                            {
                                IdUsuario = usuario.IdUsuario,
                                IdONA = usuario.IdONA,
                                Nombre = usuario.Nombre,
                                Apellido = usuario.Apellido,
                                Telefono = usuario.Telefono,
                                Email = usuario.Email,
                                IdHomologacionRol = usuario.IdHomologacionRol,
                                Estado = usuario.Estado
                                //,RazonSocial = ona.RazonSocial
                            };

                // Devolvemos el primer resultado o null si no se encuentra
                return query.FirstOrDefault();
            });
        }
        public ICollection<UsuarioDto> FindAll()
        {
            return ExecuteDbOperation(context =>
            {
                var query = from usuario in context.Usuario.AsNoTracking()
                            join homologacion in context.VwRol.AsNoTracking()
                            on usuario.IdHomologacionRol equals homologacion.IdHomologacionRol into homologacionJoin
                            from homologacion in homologacionJoin.DefaultIfEmpty()
                            join ona in context.ONA.AsNoTracking()
                            on usuario.IdONA equals ona.IdONA
                            where usuario.Estado == "A"  // Filtrar por estado "A"
                            orderby usuario.IdUsuario
                            select new UsuarioDto
                            {
                                IdUsuario = usuario.IdUsuario,
                                IdONA = usuario.IdONA,
                                Nombre = usuario.Nombre,
                                Apellido = usuario.Apellido,
                                Telefono = usuario.Telefono,
                                Email = usuario.Email,
                                Rol = homologacion.Rol,
                                Estado = usuario.Estado,
                                RazonSocial = ona.RazonSocial,
                                IdHomologacionRol = usuario.IdHomologacionRol
                            };

                return query.ToList();
            });
        }

        /// Finds a user by their email address.
        public Usuario? FindByEmail(string email) {
            return ExecuteDbOperation(context => 
                context.Usuario.AsNoTracking().FirstOrDefault(u => u.Email != null && u.Email.ToLower() == email.ToLower()));
        }

        public bool IsUniqueUser(string email)
        {
            return ExecuteDbOperation(context => 
                context.Usuario.AsNoTracking().FirstOrDefault(u => u.Email == email) == null);
        }
        public bool Create(Usuario usuario)
        {
            var clave = usuario.Clave;
            usuario.Clave = _hashService.GenerateHash(clave);
            usuario.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            usuario.IdUserModifica = usuario.IdUserCreacion;
            usuario.Estado = "A";
            return ExecuteDbOperation(context =>
            {
                context.Usuario.Add(usuario);
                if ( context.SaveChanges() >= 0 )
                {
                    SendConfirmationEmail(usuario, clave);
                    return true;
                }
                return false;
            });
        }
        public void SendConfirmationEmail(Usuario usuario, string clave)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "templates", "confirmation_access_key_template.html");

            if (File.Exists(templatePath))
            {
                string htmlBody = File.ReadAllText(templatePath);
                htmlBody = string.Format(htmlBody, usuario.Nombre, usuario.Email, clave);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.EnviarCorreoAsync(usuario.Email ?? "", "Confirmación de Recepción de Clave de Acceso al Sistema", htmlBody);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al enviar correo: {ex.Message}");
                    }
                });
            }
            else
            {
                throw new FileNotFoundException("La plantilla de correo no se encuentra en la ubicación especificada.");
            }
        }
        //public bool Update(Usuario usuario)
        //{
        //    return ExecuteDbOperation(context =>
        //    {
        //        var _exits = MergeEntityProperties(context, usuario, u => u.IdUsuario == usuario.IdUsuario);

        //        _exits.FechaModifica = DateTime.Now;
        //        _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
        //        _exits.Clave = _hashService.GenerateHash(usuario.Clave);
        //        context.Usuario.Update(_exits);
        //        return context.SaveChanges() >= 0;
        //    });
        //}
        public bool Update(Usuario usuario)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, usuario, u => u.IdUsuario == usuario.IdUsuario);

                if (_exits == null)
                {
                    return false; // Si el usuario no existe, no continuamos
                }

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

                // Consultamos la clave actual del usuario en la base de datos
                var claveActual = context.Usuario
                    .Where(u => u.IdUsuario == usuario.IdUsuario)
                    .Select(u => u.Clave)
                    .FirstOrDefault();

                // Si la clave entrante es nula o vacía, mantenemos la clave actual
                if (string.IsNullOrEmpty(usuario.Clave))
                {
                    _exits.Clave = claveActual;
                }
                else
                {
                    _exits.Clave = _hashService.GenerateHash(usuario.Clave);
                }

                context.Usuario.Update(_exits);
                return context.SaveChanges() > 0; // Si se guardaron cambios, retornará true
            });
        }

        public Result<bool> ChangePasswd(string clave, string claveNueva)
        {
            var actual = _hashService.GenerateHash(clave);
            var nueva = _hashService.GenerateHash(claveNueva);
            var idUsuario = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "0");

            var eventTrackingDto = new paAddEventTrackingDto
            {
                NombrePagina = "cambiar_clave",
                NombreControl = "btnCambiar",
                NombreAccion = "OnCambiarClave()",
                ParametroJson = JsonConvert.SerializeObject(new
                {
                    IdUsuario = idUsuario,
                    Clave = clave,
                    ClaveNueva = claveNueva
                })
            };

            return ExecuteDbOperation(context => {
                var usuario = context.Usuario.AsNoTracking().Where((c) => c.IdUsuario == idUsuario).FirstOrDefault();

                if (usuario == null)
                {
                    _eventTrackingRepository.Create(eventTrackingDto);
                    return Result<bool>.Failure("Usuario no encontrado");
                }

                var rol = context.VwRol.AsNoTracking().FirstOrDefault(c => c.IdHomologacionRol == usuario.IdHomologacionRol);
                eventTrackingDto.TipoUsuario = rol.CodigoHomologacion;
                eventTrackingDto.NombreUsuario = usuario.Nombre;
                _eventTrackingRepository.Create(eventTrackingDto);

                if (usuario.Clave != actual)
                {
                    return Result<bool>.Failure("Clave incorrecta");
                }

                usuario.Clave = nueva;
                context.Usuario.Update(usuario);
                if (context.SaveChanges() > 0)
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("Error al cambiar la clave. Intente de Nuevo");
            });
        }
    }
}