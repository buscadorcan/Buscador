using System.ComponentModel.DataAnnotations;

namespace SharedApp.Models.Dtos
{
    public class UsuarioDto
    {
        public int IdUsuario { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Nombre { get; set; }
        [Required]
        public string? Apellido { get; set; }
        [Required]
        public string? Telefono { get; set; }
        [Required]
        public string? Rol { get; set; }
        public string? Clave { get; set; }
        public string? Estado { get; set; }
    }
}
