using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
  public class PsBuscarPalabra {
    [Key]
    public string? IdEnte { get; set; }
    public string? IdVista { get; set; }
    public int IdHomologacion { get; set; }
    public string? DataEsquemaJson { get; set; }
  }
}
