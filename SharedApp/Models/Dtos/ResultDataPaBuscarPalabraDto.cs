namespace SharedApp.Models.Dtos
{
  public class ResultDataPaBuscarPalabraDto
  {
    public int? IdEsquema { get; set; }
    public int? VistaPK { get; set; }
    public int? IdEsquemaData { get; set; }
    public List<ColumnaEsquemaDto>? DataEsquemaJson { get; set; }
  }
}
