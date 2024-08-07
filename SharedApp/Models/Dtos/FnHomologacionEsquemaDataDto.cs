namespace SharedApp.Models.Dtos
{
    public class FnHomologacionEsquemaData
    {
      public int IdCanDataSet { get; set; }
      public int IdHomologacionEsquema { get; set; }
      public string? DataEsquemaJson { get; set; }
    }
    public class FnHomologacionEsquemaDataDto
    {
        public int IdCanDataSet { get; set; }
        public int IdHomologacionEsquema { get; set; }
        public List<ColumnaEsquema>? DataEsquemaJson { get; set; }
    }
    public class ColumnaEsquema
    {
        public int IdHomologacion { get; set; }
        public string? Data { get; set; }
    }
}
