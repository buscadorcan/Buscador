using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vwFiltro")]
    public class VwFiltro : IVwHomologacion
    {
        public string? NombreFiltro { get; set; }
    }
}