namespace ClientApp.Models
{
    public class VwHomologacion
    {
        public int IdHomologacion { get; set; }
        public int? IdHomologacionGrupo { get; set; }
        public string? MostrarWeb { get; set; }
        public string? TooltipWeb { get; set; }
        public int MostrarWebOrden { get; set; }
        public int AnchoColumna { get; set; }
        public string? MascaraDato { get; set; }
        public string? SiNoHayDato { get; set; }
        public string? CodigoHomologacion { get; set; }
        public string? NombreHomologado { get; set; }
        public string? InfoExtraJson { get; set; }
        public string? CustomMostrarWeb { get; set; }
    }
}