using System.ComponentModel.DataAnnotations;

namespace SharedApp.Dtos
{
  public class HomologacionEsquemaDto
  {
    public int IdEsquema { get; set; }
    public int IdHomologacionEsquema { get; set; }
    public string? IdVistaNombre { get; set; }
    [Required]
    public string? EsquemaJson { get; set; }
    [Required]
    public int MostrarWebOrden { get; set; }
    [Required]
    public string? MostrarWeb { get; set; }
    [Required]
    public string? TooltipWeb { get; set; }
    [Required]
    public string? VistaNombre { get; set; }
    public string? DataTipo { get; set; }
    [Required]
    public string? EsquemaVista { get; set; } = "";

    }
}
