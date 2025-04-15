using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
  public class EsquemaVistaColumna : BaseEntity
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEsquemaVistaColumna { get; set; }
    [Required]
    public int IdEsquemaVista { get; set; }
    [Required]
    public int ColumnaEsquemaIdH { get; set; }
    [Required]
    public string? ColumnaEsquema { get; set; }
    [Required]
    public string? ColumnaVista { get; set; }
    [Required]
    public bool ColumnaVistaPK { get; set; }
    [Required]
    public string? Estado { get; set; }
    [ForeignKey("IdEsquemaVista")]
    public EsquemaVista? EsquemaVista { get; set; }
  }
}
