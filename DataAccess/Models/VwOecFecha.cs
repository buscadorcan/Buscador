using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_OecFecha")]
    public class VwOecFecha
    {
        public string Fecha { get; set; } = "";
        public int Organizacion { get; set; }
    }
}
