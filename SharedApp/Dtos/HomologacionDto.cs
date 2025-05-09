using System.ComponentModel.DataAnnotations;

namespace SharedApp.Dtos
{
    public class HomologacionDto
    {
        public int IdHomologacion { get; set; }
        public int? IdHomologacionGrupo { get; set; }
        public int? IdHomologacionFiltro { get; set; }
        public int MostrarWebOrden { get; set; }
        public string? MostrarWeb { get; set; }
        public string? Mostrar { get; set; }
        public string? TooltipWeb { get; set; }
        public string? MascaraDato { get; set; }
        public string? SiNoHayDato { get; set; }
        public string? NombreHomologado { get; set; }
        public string? InfoExtraJson { get; set; }
        public string? CodigoHomologacion { get; set; }
        public int AnchoColumna { get; set; }
        public string? CustomMostrarWeb { get; set; }
        public string? NombreFiltro { get; set; }
        public string? Indexar { get; set; }
        public string? Estado { get; set; }
        
    }
}
