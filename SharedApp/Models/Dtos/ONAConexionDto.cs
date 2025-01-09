using System.ComponentModel.DataAnnotations;

namespace SharedApp.Models.Dtos
{
    public class ONAConexionDto
    {
        [Required(ErrorMessage = "El campo es obligatorio.")]
        public int IdONA { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]
        public string? Host { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]
        [Range(1000, 9999, ErrorMessage = "El puerto debe tener 4 dígitos.")]
        public int Puerto { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]
        public string? Usuario { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]
        public string? Contrasenia { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]
        public string? BaseDatos { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]
        public string? OrigenDatos { get; set; }
        [Required(ErrorMessage = "El campo es obligatorio.")]
        public string? Migrar { get; set; }

        public string? Estado { get; set; }
    }
}