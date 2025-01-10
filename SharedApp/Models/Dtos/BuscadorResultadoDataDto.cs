namespace SharedApp.Models.Dtos
{
    public class BuscadorResultadoData
    {
      public string? IdEnte { get; set; }
      public int IdEsquema { get; set; }
      public string? IdVista { get; set; }
      public string? DataEsquemaJson { get; set; }
    }
    public class BuscadorResultadoDataDto
    {
        public string? IdEnte { get; set; }
        public int IdEsquema { get; set; }
        public string? IdVista { get; set; }
        public List<ColumnaEsquema>? DataEsquemaJson { get; set; }
    }
}
