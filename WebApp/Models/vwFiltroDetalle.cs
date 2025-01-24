using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class vwFiltroDetalle
    {
        [Key]
        public int IdHomologacion { get; set; }
        public string? MostrarWeb { get; set; }
        public string? CodigoHomologacion { get; set; }
    }
}
