using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class ONAConexion : BaseEntity
    {
        [Key]
        public int IdONA { get; set; }
        [Required]
        public string? Host { get; set; }
        [Required]
        public int Puerto { get; set; }
        [Required]
        public string? Usuario { get; set; }
        [Required]
        public string? Contrasenia { get; set; }
        [Required]
        public string? BaseDatos { get; set; }
        [Required]
        public string? OrigenDatos { get; set; }
        [Required]
        public string? Migrar { get; set; }
        [Required]
        public string? Estado { get; set; }
        [Required]
        public int? IdUserCreacion { get; set; }

        [Required]
        public int? IdUserModifica { get; set; }

        [ForeignKey("IdONA")]
        public virtual ONA? ONA { get; set; }
    }
}
