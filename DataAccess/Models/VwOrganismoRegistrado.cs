using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vw_OrganismoRegistrado")]
    public class VwOrganismoRegistrado
    {
        public string Fecha { get; set; } = "";
        public int Profesionales { get; set; }
    }
}
