namespace SharedApp.Dtos
{
  public class CanDataSetDto
  {
    public int IdCanDataSet { get; set; }
    public int? IdHomologacionSistema { get; set; }
    public int? IdConexion { get; set; }
    public string? IdEnte { get; set; }
    public string? IdVista { get; set; }
    public string? DataEsquemaJson { get; set; }
  }
}
