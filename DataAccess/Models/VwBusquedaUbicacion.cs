using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_BusquedaUbicacion")]
    public class VwBusquedaUbicacion
    {
        public string Pais { get; set; } = "";
        public string Ciudad { get; set; } = "";
        public int Busqueda { get; set; }
    }
}
