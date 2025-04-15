using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vwHomologacion")]
    public class VwHomologacion
    {
        [Key]
        public int IdHomologacion { get; set; }
        public int? IdHomologacionGrupo { get; set; }
        public string? Indexar { get; set; }
        public string? MostrarWeb { get; set; }
        public string? TooltipWeb { get; set; }
        public int? MostrarWebOrden { get; set; }
        public string? MascaraDato { get; set; }
        public string? SiNoHayDato { get; set; }
        public string? NombreHomologado { get; set; }
        public string? CodigoHomologacion { get; set; }
        public string? CodigoHomologacionKEY { get; set; }
    }
}
