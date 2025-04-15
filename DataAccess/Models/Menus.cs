using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class Menus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMenuRol { get; set; }
        public int? IdHRol { get; set; }
        public string? Rol { get; set; }
        public int? IdHMenu { get; set; }
        public string? Menu { get; set; }
        [Required]
        public string? Estado { get; set; }
        [Required]
        public DateTime? FechaCreacion { get; set; } = DateTime.Now;
    }
}
