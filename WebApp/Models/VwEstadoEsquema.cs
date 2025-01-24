using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vw_EstadoEsquema")]
    public class VwEstadoEsquema
    {
        public string Esquema { get; set; } = "";
        public string Estado { get; set; } = "";
        public int Organizacion { get; set; }
    }
}
