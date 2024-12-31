using System.ComponentModel.DataAnnotations;

namespace SharedApp.Models.Dtos
{
  public class UsuarioDto
  {
    public int IdUsuario { get; set; }
    [Required]
    public int IdHomologacionRol { get; set; }
    [Required]
    public int IdONA { get; set; }
    [Required(ErrorMessage = "El Nombre del Usuario es obligatorio.")]
    public string? Nombre { get; set; }
    [Required(ErrorMessage = "El Apellido del Usuario es obligatorio.")]
    public string? Apellido { get; set; }
    [Required]
    public string? Telefono { get; set; }

    [Required]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "La clave es obligatoria.")]
    public string? Clave { get; set; }
    [Required]
    public string? Estado { get; set; }
    public string Rol { get; set; }
    public string RazonSocial { get; set; }
    }
}
