using System.ComponentModel.DataAnnotations;

namespace SharedApp.Models.Dtos
{
  public class HomologacionEsquemaDto
  {
    public int IdHomologacionEsquema { get; set; }
    public int IdVistaNombre { get; set; }
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
  }
}
