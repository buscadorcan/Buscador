using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_ActualizacionONA")]
    public class VwActualizacionONA
    {
        public string Fecha { get; set; } = "";
        public string ONA { get; set; } = "";
        public int Actualizaciones { get; set; }
    }
}
