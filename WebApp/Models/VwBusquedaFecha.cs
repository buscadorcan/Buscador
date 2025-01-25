using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vw_BusquedaFecha")]
    public class VwBusquedaFecha
    {
        public string Fecha { get; set; } = "";
        public int Busqueda { get; set; }
    }
}
