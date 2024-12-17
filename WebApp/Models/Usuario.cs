using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  public class Usuario : BaseEntity
  {
    [Key]
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
    [Required]
    public string? Clave { get; set; }
    [Required]
    public string? Estado { get; set; }
    [ForeignKey("IdHomologacionRol")]
    public Homologacion? Homologacion { get; set; }
  }
}
