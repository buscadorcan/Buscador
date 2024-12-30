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
    public string? Ciudad { get; set; }
    public string? Correo { get; set; }
    public string? Direccion { get; set; }
    public string? PaginaWeb { get; set; }
    public string? Telefono { get; set; }
    public string? UrlIcono { get; set; }
    public string? UrlLogo { get; set; }
    [Required]
    public string? InfoExtraJson { get; set; }
    [Required]
    public string? Estado { get; set; }
    public int? IdHomologacionPais { get; set; }    
    }
}
