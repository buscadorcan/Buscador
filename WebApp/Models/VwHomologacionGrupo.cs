using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vwHomologacionGrupo")]
    public class VwHomologacionGrupo
    {
        [Key]
        public int IdHomologacion { get; set; }
        public int? IdHomologacionGrupo { get; set; }
        public string? MostrarWeb { get; set; }
        public string? TooltipWeb { get; set; }
        public int? MostrarWebOrden { get; set; }
        public string? CodigoHomologacion { get; set; }
        public string? Estado { get; set; }
    }
}
