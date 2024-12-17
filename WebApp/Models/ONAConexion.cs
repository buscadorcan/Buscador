using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  public class ONAConexion : BaseEntity
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdONA { get; set; }
    [Required]
    public string? Host { get; set; }
    [Required]
    public int Puerto { get; set; }
    [Required]
    public string? Usuario { get; set; }
    [Required]
    public string? Contrasenia { get; set; }
    [Required]
    public string? BaseDatos { get; set; }
    [Required]
    public string? OrigenDatos { get; set; }
    [Required]
    public string? Migrar { get; set; }
    [Required]
    public string? Estado { get; set; }
  }
}
