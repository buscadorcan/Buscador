namespace SharedApp.Models.Dtos
{
    public class BuscadorResultadoData
    {
      public int? IdEsquemaData { get; set; }
      public int IdEsquema { get; set; }
      public string? VistaPK { get; set; }
      public string? DataEsquemaJson { get; set; }
    }
    public class BuscadorResultadoDataDto
    {
        public int? IdEsquemaData { get; set; }
        public int IdEsquema { get; set; }
        public string? VistaPK { get; set; }
        public List<ColumnaEsquema>? DataEsquemaJson { get; set; }
    }
}
