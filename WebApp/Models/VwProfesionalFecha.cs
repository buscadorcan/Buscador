using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vw_ProfesionalFecha")]
    public class VwProfesionalFecha
    {
        public string Fecha { get; set; } = "";
        public int Profesionales { get; set; }
    }
}
