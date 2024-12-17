using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  public class ONA : BaseEntity
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdONA { get; set; }
    [Required]
    public string? RazonSocial { get; set; }
    [Required]
    public string? Siglas { get; set; }
    [Required]
    public string? Pais { get; set; }
    [Required]
    public string? Ciudad { get; set; }
    [Required]
    public string? Correo { get; set; }
    [Required]
    public string? Direccion { get; set; }
    [Required]
    public string? PaginaWeb { get; set; }
    [Required]
    public string? Telefono { get; set; }
    [Required]
    public string? UrlIcono { get; set; }
    [Required]
    public string? UrlLogo { get; set; }
    [Required]
    public string? InfoExtraJson { get; set; }
    [Required]
    public string? Estado { get; set; }
  }
}
