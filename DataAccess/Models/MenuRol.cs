using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class MenuRol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdMenuRol { get; set; }
        public int? IdHRol { get; set; }
        public int? IdHMenu { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaCreacion { get; set; } = DateTime.Now;
    }
}
