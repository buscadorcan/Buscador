using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_AcreditacionEsquema")]
    public class VwAcreditacionEsquema
    {
        public string Esquema { get; set; } = "";
        public int Organizacion { get; set; }
    }
}
