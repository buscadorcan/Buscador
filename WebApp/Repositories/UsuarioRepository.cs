using WebApp.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using WebApp.Service.IService;
using SharedApp.Models.Dtos;
using WebApp.Models;

namespace WebApp.Repositories
{
  /// <summary>
  /// Repositorio para la gestión de usuarios, implementa las operaciones definidas en <see cref="IUsuarioRepository"/>.
  /// </summary>
  public class UsuarioRepository : BaseRepository, IUsuarioRepository
  {
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IHashService _hashService;
    private readonly IPasswordService _passwordService;

    /// <summary>
    /// Constructor para inicializar las dependencias del repositorio.
    /// </summary>
    /// <param name="jwtService">Servicio para la generación y validación de JWT.</param>
    /// <param name="emailService">Servicio para el envío de correos electrónicos.</param>
    /// <param name="hashService">Servicio para la generación de hashes de contraseñas.</param>
    /// <param name="passwordService">Servicio para la generación de contraseñas temporales.</param>
    /// <param name="logger">Instancia del logger para registrar eventos.</param>
    /// <param name="sqlServerDbContextFactory">Fábrica para el contexto de base de datos SQL Server.</param>
    public UsuarioRepository(
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

    /// <inheritdoc />
    public Usuario? FindById(int idUsuario)
    {
      return ExecuteDbOperation(context => context.Usuario.AsNoTracking().FirstOrDefault(c => c.IdUsuario == idUsuario));
    }

    /// <inheritdoc />
    public ICollection<Usuario> FindAll()
    {
      return ExecuteDbOperation(context =>
      context.Usuario.AsNoTracking().OrderBy(c => c.IdUsuario).ToList());
    }

    /// <inheritdoc />
    public bool IsUniqueUser(string email)
    {
      return ExecuteDbOperation(context =>
      context.Usuario.AsNoTracking().FirstOrDefault(u => u.Email == email) == null);
    }

    /// <inheritdoc />
    public bool Create(Usuario usuario)
    {
      usuario.Clave = _hashService.GenerateHash(usuario.Clave);
      usuario.IdUserCreacion = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");
      usuario.IdUserModifica = usuario.IdUserCreacion;

      return ExecuteDbOperation(context =>
      {
        context.Usuario.Add(usuario);
        return context.SaveChanges() >= 0;
      });
    }

    /// <inheritdoc />
    public bool Update(Usuario usuario)
    {
      return ExecuteDbOperation(context =>
      {
        var userExits = MergeEntityProperties(context, usuario, u => u.IdUsuario == usuario.IdUsuario);
        userExits.FechaModifica = DateTime.Now;
        userExits.IdUserModifica = _jwtService.GetUserIdFromToken(_jwtService.GetTokenFromHeader() ?? "");

        if (!string.IsNullOrWhiteSpace(usuario.Clave))
        {
          userExits.Clave = _hashService.GenerateHash(usuario.Clave);
        }

        context.Usuario.Update(userExits);
        return context.SaveChanges() >= 0;
      });
    }

    /// <inheritdoc />
    public UsuarioAutenticacionRespuestaDto Login(UsuarioAutenticacionDto usuarioAutenticacionDto)
    {
      return ExecuteDbOperation(context =>
      {
        var passwordEncriptado = _hashService.GenerateHash(usuarioAutenticacionDto.Clave);
        var usuario = context.Usuario.AsNoTracking().Include(u => u.Homologacion).FirstOrDefault(u =>
          u.Email != null &&
          usuarioAutenticacionDto.Email != null &&
          u.Email.ToLower() == usuarioAutenticacionDto.Email.ToLower() &&
          u.Clave == passwordEncriptado);

        if (usuario == null)
        {
          return new UsuarioAutenticacionRespuestaDto();
        }

        return new UsuarioAutenticacionRespuestaDto
        {
          Token = _jwtService.GenerateJwtToken(usuario.IdUsuario),
          Usuario = new UsuarioDto {
            IdUsuario = usuario.IdUsuario,
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Telefono = usuario.Telefono,
            IdHomologacionRol = usuario.IdHomologacionRol ?? 0,
            IdONA = usuario.IdONA ?? 0,
            Rol = usuario.Homologacion?.NombreHomologado
          }
        };
      });
    }

    /// <inheritdoc />
    public async Task<bool> RecoverAsync(UsuarioRecuperacionDto usuarioRecuperacionDto)
    {
      return await ExecuteDbOperation(async context => {
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
