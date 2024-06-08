namespace WebApp.Models.Dtos
{
    public class FnHomologacionEsquemaData
    {
      public int IdDataLakeOrganizacion { get; set; }
      public int IdHomologacionEsquema { get; set; }
      public string? DataEsquemaJson { get; set; }
    }
    public class FnHomologacionEsquemaDataDto
    {
        public int IdDataLakeOrganizacion { get; set; }
        public int IdHomologacionEsquema { get; set; }
        public List<ColumnaEsquema> DataEsquemaJson { get; set; }
    }
    public class ColumnaEsquema
    {
        public int IdHomologacion { get; set; }
        public string Data { get; set; }
    }
}
