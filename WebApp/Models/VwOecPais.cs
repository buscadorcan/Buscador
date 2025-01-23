using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vw_OecPais")]
    public class VwOecPais
    {
        public string? Pais { get; set; } = "";
        public int? Organizacion { get; set; }
    }
}
