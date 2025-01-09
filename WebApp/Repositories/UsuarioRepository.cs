using WebApp.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using WebApp.Service.IService;
using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories
{
    public class UsuarioRepository : BaseRepository, IUsuarioRepository
    {
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IHashService _hashService;
        private readonly IPasswordService _passwordService;
        public UsuarioRepository (
            IJwtService jwtService,
            IEmailService emailService,
            IHashService hashService,
            IPasswordService passwordService,
            ILogger<UsuarioRepository> logger,
            ISqlServerDbContextFactory sqlServerDbContextFactory
        ) : base(sqlServerDbContextFactory, logger)
        {
            _jwtService = jwtService;
            _emailService = emailService;
            _hashService = hashService;
            _passwordService = passwordService;
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

        public bool IsUniqueUser(string email)
        {
            return ExecuteDbOperation(context => 
                context.Usuario.AsNoTracking().FirstOrDefault(u => u.Email == email) == null);
        }
        public bool Create(Usuario usuario)
        {
            usuario.Clave = _hashService.GenerateHash(usuario.Clave);
            usuario.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
            usuario.IdUserModifica = usuario.IdUserCreacion;
            usuario.Estado = "A";
            return ExecuteDbOperation(context =>
            {
                context.Usuario.Add(usuario);
                return context.SaveChanges() >= 0;
            });
        }
        public bool Update(Usuario usuario)
        {
            return ExecuteDbOperation(context =>
            {
                var _exits = MergeEntityProperties(context, usuario, u => u.IdUsuario == usuario.IdUsuario);

                _exits.FechaModifica = DateTime.Now;
                _exits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
                _exits.Clave = _hashService.GenerateHash(usuario.Clave);
                context.Usuario.Update(_exits);
                return context.SaveChanges() >= 0;
            });
        }
        //public bool Update(Usuario usuario)
        //{
        //    return ExecuteDbOperation(context => {
        //        var userExits = MergeEntityProperties(context, usuario, u => u.IdUsuario == usuario.IdUsuario);
        //        userExits.FechaModifica = DateTime.Now;
        //        userExits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

        //        if (!string.IsNullOrWhiteSpace(usuario.Clave)) {
        //            userExits.Clave = _hashService.GenerateHash(usuario.Clave);
        //        }

        //        context.Usuario.Update(userExits);
        //        return context.SaveChanges() >= 0;
        //    });
        //}
        public UsuarioAutenticacionRespuestaDto Login(UsuarioAutenticacionDto usuarioAutenticacionDto)
        {

            return ExecuteDbOperation(context =>
            {
                // Generar el hash de la contraseña
                var passwordEncriptado = _hashService.GenerateHash(usuarioAutenticacionDto.Clave);

                // Verificar que el email no sea nulo
                if (usuarioAutenticacionDto.Email == null)
                {
                    return new UsuarioAutenticacionRespuestaDto();
                }

                //Buscar el usuario en la base de datos
                var usuario = context.Usuario.AsNoTracking().FirstOrDefault(u =>
                    u.Email != null &&
                    u.Email.ToLower() == usuarioAutenticacionDto.Email.ToLower() &&
                    u.Clave == passwordEncriptado);


                // Si no se encuentra el usuario, retornar un objeto vacío
                if (usuario == null)
                {
                    return new UsuarioAutenticacionRespuestaDto();
                }

                // Crear el token JWT
                var token = _jwtService.GenerateJwtToken(usuario.IdUsuario);

                // Crear el objeto UsuarioDto
                var usuarioDto = new UsuarioDto
                {
                    IdUsuario = usuario.IdUsuario,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Apellido = usuario.Apellido,
                    Telefono = usuario.Telefono,
                    IdHomologacionRol = usuario.IdHomologacionRol,
                    IdONA = usuario.IdONA,
                };

                // Obtener el rol del usuario desde la vista VwRol usando IdHomologacionRol
                var rol = context.VwRol.AsNoTracking().FirstOrDefault(r =>
                    r.IdHomologacionRol == usuario.IdHomologacionRol);

                // Agregar el rol a la respuesta
                var rolDto = rol != null ? new VwRolDto
                {
                    IdHomologacionRol = rol.IdHomologacionRol,
                    Rol = rol.Rol,
                    CodigoHomologacion = rol.CodigoHomologacion
                } : null;

                // Retornar la respuesta con el token y el usuario
                return new UsuarioAutenticacionRespuestaDto
                {
                    Token = token,
                    Usuario = usuarioDto,
                    Rol = rolDto
                };
            });
        }
        public async Task<bool> RecoverAsync(UsuarioRecuperacionDto usuarioRecuperacionDto)
        {
            return await ExecuteDbOperation(async context =>
            {
                var usuario = context.Usuario.AsNoTracking().FirstOrDefault(u =>
                    u.Email != null &&
                    usuarioRecuperacionDto.Email != null &&
                    u.Email.ToLower() == usuarioRecuperacionDto.Email.ToLower());

                if (usuario == null)
                {
                    return false;
                }

                string clave = _passwordService.GenerateTemporaryPassword(8);
                usuario.Clave = _hashService.GenerateHash(clave);

                context.Usuario.Update(usuario);
                var result = context.SaveChanges() >= 0;
                Console.WriteLine($"Envío de Clave Temporal Su nueva clave temporal es: {clave}");

                await _emailService.EnviarCorreoAsync(usuario.Email ?? "", "Envío de Clave Temporal", $"Su nueva clave temporal es: {clave}");

                return result;
            });
        }
    }
}