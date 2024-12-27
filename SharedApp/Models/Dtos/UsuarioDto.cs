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
    [Required]
    public string? Nombre { get; set; }
    [Required]
    public string? Apellido { get; set; }
    [Required]
    public string? Telefono { get; set; }
    [Required]
    public string? Email { get; set; }
    public string? Clave { get; set; }
    [Required]
    public string? Estado { get; set; }
    public string Rol { get; set; }
    public string RazonSocial { get; set; }
    }
}
