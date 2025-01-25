using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    [Table("vw_OrganismoActividad")]
    public class VwOrganismoActividad
    {
        public string Organismos { get; set; } = "";
        public int Consultas { get; set; }
    }
}
