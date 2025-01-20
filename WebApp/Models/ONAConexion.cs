using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class ONAConexion : BaseEntity
    {
        [Key]
        public int IdONA { get; set; }
        
        public string? Host { get; set; }
        
        public int? Puerto { get; set; }
        
        public string? Usuario { get; set; }
        
        public string? Contrasenia { get; set; }
        
        public string? BaseDatos { get; set; }
        
        public string? OrigenDatos { get; set; }
        
        public string? Migrar { get; set; }
        
        public string? Estado { get; set; }
        
        public int? IdUserCreacion { get; set; }

        public int? IdUserModifica { get; set; }

        public DateTime? FechaCreacion { get; set; }

        public DateTime? FechaModifica { get; set; }

        [ForeignKey("IdONA")]
        public virtual ONA? ONA { get; set; }
    }
}
