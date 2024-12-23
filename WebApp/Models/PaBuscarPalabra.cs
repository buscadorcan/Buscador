using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
  public class PaBuscarPalabra {
    [Key]
    public int? IdEsquema { get; set; }
    public int? VistaPK { get; set; }
    public int? IdEsquemaData { get; set; }
    public string? DataEsquemaJson { get; set; }
  }
}
