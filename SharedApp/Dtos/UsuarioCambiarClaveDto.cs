using System.ComponentModel.DataAnnotations;

namespace SharedApp.Dtos
{
    public class UsuarioCambiarClaveDto
    {
        [Required(ErrorMessage = "La clave es obligatoria.")]
        public string? Clave { get; set; } = "";

        [Required(ErrorMessage = "La nueva clave es obligatoria.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,11}$", ErrorMessage = "La clave debe tener entre 6 y 11 caracteres, con al menos una mayúscula, una minúscula y un número.")]
        public string? ClaveNueva { get; set; } = "";
    }
}