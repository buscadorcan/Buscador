using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  public class EsquemaData
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEsquemaData { get; set; }
    [Required]
    public int IdEsquemaVista { get; set; }
    [Required]
    public string VistaFK { get; set; } = "";
    [Required]
    public string VistaPK { get; set; } = "";
    [Required]
    public string DataEsquemaJson { get; set; } = "";
    [Required]
    public DateTime DataFecha { get; set; } = DateTime.Now;
  }
}