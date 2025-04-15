using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Homologacion : BaseEntity
    {
        [Key]
        public int IdHomologacion { get; set; }
        public int? IdHomologacionGrupo { get; set; } = 0;

        public int? IdHomologacionFiltro { get; set; } = null;

        [Required]
        public string? Mostrar { get; set; } = "";
        [Required]
        public int MostrarWebOrden { get; set; } = 0;
        [Required]
        public string? MostrarWeb { get; set; } = "";
        [Required]
        public string? TooltipWeb { get; set; } = "";
        [Required]
        public string? MascaraDato { get; set; } = "";
        [Required]
        public string? SiNoHayDato { get; set; } = "";
        [Required]
        public string? InfoExtraJson { get; set; } = "";
        [Required]
        public string? CodigoHomologacion { get; set; } = "";
        [Required]
        public string? NombreHomologado { get; set; } = "";
        [Required]
        public string? Estado { get; set; } = "";
        [Required]
        public string? Indexar { get; set; } = "";
       
    }
}
