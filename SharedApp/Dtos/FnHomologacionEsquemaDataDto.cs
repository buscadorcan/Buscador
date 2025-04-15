namespace SharedApp.Dtos
{
    public class FnHomologacionEsquemaData
    {
      public int IdEsquemaData { get; set; }
      public int IdEsquema { get; set; }
      public string? DataEsquemaJson { get; set; }
    }
    public class FnHomologacionEsquemaDataDto
    {
        public int IdEsquemaData { get; set; }
        public int IdEsquema { get; set; }
        public List<ColumnaEsquema>? DataEsquemaJson { get; set; }
    }
    public class FnEsquemaDataBuscado
    {
        public int IdEsquemaData { get; set; }
        public int IdEsquema { get; set; }
        public string? DataEsquemaJson { get; set; }
    }

    public class FnEsquemaDataBuscadoDto
    {
        public int IdEsquemaData { get; set; }
        public int IdEsquema { get; set; }
        public List<ColumnaEsquema>? DataEsquemaJson { get; set; }
    }
    public class ColumnaEsquema
    {
        public int IdHomologacion { get; set; }
        public string? Data { get; set; }
    }


}
