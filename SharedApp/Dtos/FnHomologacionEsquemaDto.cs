using System.ComponentModel.DataAnnotations;

namespace SharedApp.Dtos
{
  public class FnHomologacionEsquemaDto
  {
    public int IdHomologacionEsquema { get; set; }
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
  }
}
