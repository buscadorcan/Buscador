using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_AcreditacionOna")]
    public class VwAcreditacionOna
    {
        public string Pais { get; set; } = "";
        public string ONA { get; set; } = "";
        public int Organizacion { get; set; }
    }
}
