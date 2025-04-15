using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_ProfesionalCalificado")]
    public class VwProfesionalCalificado
    {
        public string Calificacion { get; set; } = "";
        public int Profesionales { get; set; }
    }
}
