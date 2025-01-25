using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vw_ProfesionalOna")]
    public class VwProfesionalOna
    {
        public string Ona { get; set; } = "";
        public int Profesionales { get; set; }
    }
}
