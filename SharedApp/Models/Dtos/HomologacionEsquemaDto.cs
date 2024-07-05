namespace SharedApp.Models.Dtos
{
  public class HomologacionEsquemaDto
  {
    public int IdHomologacionEsquema { get; set; }
    public string? EsquemaJson { get; set; }
    public int MostrarWebOrden { get; set; }
    public string? MostrarWeb { get; set; }
    public string? TooltipWeb { get; set; }
    public string? VistaNombre { get; set; }
  }
}
