using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vw_ProfesionalEsquema")]
    public class VwProfesionalEsquema
    {
        public string Esquema { get; set; } = "";
        public int Profesionales { get; set; }
    }
}
