using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vw_OrganizacionEsquema")]
    public class VwOrganizacionEsquema
    {
        public string Esquema { get; set; } = "";
        public int Organizacion { get; set; }
    }
}
