using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_EsquemaPais")]
    public class VwEsquemaPais
    {
        public string Esquema { get; set; } = "";
        public string Pais { get; set; } = "";
        public int Organizacion { get; set; }
    }
}
