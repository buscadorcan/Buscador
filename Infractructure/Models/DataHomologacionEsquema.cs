namespace Infractruture.Models
{
    /// <summary>
    /// Modelo de datos para el resultado de la busqueda general.
    /// </summary>
    public class ResultDataHomologacionEsquema
    {
        public List<DataHomologacionEsquema>? Data { get; set; }
        public int TotalCount { get; set; }
    }
    public class DataHomologacionEsquema
    {
        public int IdDataLakeOrganizacion { get; set; }
        public int IdHomologacionEsquema { get; set; }
        public List<ColumnaEsquema>? DataEsquemaJson { get; set; }
    }

    public class DataEsquemaDatoBuscar
    {
        public int IdDataLakeOrganizacion { get; set; }
        public int IdHomologacionEsquema { get; set; }
        public List<ColumnaEsquema>? DataEsquemaJson { get; set; }
    }
    public class ColumnaEsquema
    {
        public int IdHomologacion { get; set; }
        public string? Data { get; set; }
    }
}