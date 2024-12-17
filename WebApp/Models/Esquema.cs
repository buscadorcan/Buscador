using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  public class Esquema : BaseEntity
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEsquema { get; set; }
    [Required]
    public int MostrarWebOrden { get; set; }
    [Required]
    public string MostrarWeb { get; set; } = "";
    [Required]
    public string TooltipWeb { get; set; } = "";
    [Required]
    public string EsquemaVista { get; set; } = "";
    [Required]
    public string EsquemaJson { get; set; } = "";
    [Required]
    public string Estado { get; set; } = "A";
  }
}
