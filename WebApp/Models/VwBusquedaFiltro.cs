using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vw_BusquedaFiltro")]
    public class VwBusquedaFiltro
    {
        public string FiltroPor { get; set; } = "";
        public int Busqueda { get; set; }
    }
}
