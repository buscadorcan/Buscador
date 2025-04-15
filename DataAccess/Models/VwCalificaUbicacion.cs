using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_CalificaUbicacion")]
    public class VwCalificaUbicacion
    {
        public string Pais { get; set; } = "";
        public int Calificados { get; set; }
    }
}
