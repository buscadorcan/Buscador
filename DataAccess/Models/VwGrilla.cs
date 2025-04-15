using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
  [Table("vwGrilla")]
  public class VwGrilla
  {
    [Key]
    public int IdHomologacion { get; set; }
    public string? MostrarWeb { get; set; }
    public string? TooltipWeb { get; set; }
    public int MostrarWebOrden { get; set; }
  }
}
