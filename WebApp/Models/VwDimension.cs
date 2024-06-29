using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vwDimension")]
    public class VwDimension : IVwHomologacion
    {
        public string? NombreHomologado { get; set; }
        public string? CustomMostrarWeb { get; set; }
    }
}