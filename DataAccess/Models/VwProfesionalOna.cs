using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_ProfesionalOna")]
    public class VwProfesionalOna
    {
        public string Ona { get; set; } = "";
        public int Profesionales { get; set; }
    }
}
