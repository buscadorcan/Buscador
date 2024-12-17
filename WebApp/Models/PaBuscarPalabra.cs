using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
  public class PaBuscarPalabra {
    [Key]
    public string? IdEnte { get; set; }
    public string? IdVista { get; set; }
    public int IdHomologacion { get; set; }
    public string? DataEsquemaJson { get; set; }
  }
}
