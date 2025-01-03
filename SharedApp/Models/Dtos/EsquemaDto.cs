using System.ComponentModel.DataAnnotations;

namespace SharedApp.Models.Dtos
{
    public class EsquemaDto
    {
        public int IdEsquema { get; set; }
        public int MostrarWebOrden { get; set; }
        public string? MostrarWeb { get; set; }
        public string? TooltipWeb { get; set; }
        public string? EsquemaVista { get; set; }
        public string? EsquemaJson { get; set; }
        public string? Estado { get; set; }
    }
}
