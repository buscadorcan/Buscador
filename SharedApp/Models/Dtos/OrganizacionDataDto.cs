namespace SharedApp.Models.Dtos
{
  public class OrganizacionDataDto
  {
    public int IdOrganizacionData { get; set; }
    public int? IdHomologacionSistema { get; set; }
    public int? IdConexion { get; set; }
    public string? IdOrganizacion { get; set; }
    public string? IdVista { get; set; }
    public string? DataEsquemaJson { get; set; }
  }
}
