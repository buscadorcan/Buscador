namespace SharedApp.Models.Dtos
{
  public class ResultDataPaBuscarPalabraDto
  {
    public string? IdEnte { get; set; }
    public int IdHomologacion { get; set; }
    public string? IdVista { get; set; }
    public List<ColumnaEsquemaDto>? DataEsquemaJson { get; set; }
  }
}
