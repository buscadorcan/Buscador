namespace SharedApp.Models.Dtos
{
    public class BuscadorResultadoData
    {
      public string? IdOrganizacion { get; set; }
      public int IdHomologacionEsquema { get; set; }
      public string? IdVista { get; set; }
      public string? DataEsquemaJson { get; set; }
    }
    public class BuscadorResultadoDataDto
    {
        public string? IdOrganizacion { get; set; }
        public int IdHomologacionEsquema { get; set; }
        public string? IdVista { get; set; }
        public List<ColumnaEsquema>? DataEsquemaJson { get; set; }
    }
}
