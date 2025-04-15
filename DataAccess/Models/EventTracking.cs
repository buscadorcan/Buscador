using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
    public class EventTracking
    {
        [Key]
        public int IdEventTracking { get; set; }

        [Required]
        public string CodigoHomologacionRol { get; set; } = string.Empty;

        [Required]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        public string CodigoHomologacionMenu { get; set; } = string.Empty;

        [Required]
        public string NombreControl { get; set; } = string.Empty;

        [Required]
        public string NombreAccion { get; set; } = string.Empty;

        [Required]
        public string UbicacionJson { get; set; } = "{}";

        [Required]
        public string ParametroJson { get; set; } = "{}";

        [Required]
        public string ErrorTracking { get; set; } = string.Empty;

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
