using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_ProfesionalEsquema")]
    public class VwProfesionalEsquema
    {
        public string Esquema { get; set; } = "";
        public int Profesionales { get; set; }
    }
}
