using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  [Table("HomologacionEsquema")]
  public class HomologacionEsquema : BaseEntity
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdHomologacionEsquema { get; set; }
    public string? IdVistaNombre { get; set; }
    [Required]
    public string? EsquemaJson { get; set; }
    [Required]
    public int MostrarWebOrden { get; set; }
    [Required]
    public string? MostrarWeb { get; set; }
    [Required]
    public string? VistaNombre { get; set; }
    public string? DataTipo { get; set; }
    public string? TooltipWeb { get; set; }
  }
}