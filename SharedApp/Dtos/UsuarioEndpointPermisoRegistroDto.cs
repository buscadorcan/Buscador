using System.ComponentModel.DataAnnotations;

namespace SharedApp.Dtos
{
    public class UsuarioEndpointPermisoRegistroDto
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int IdUsuario { get; set; }
        [Required(ErrorMessage = "El endpoint es obligatorio")]
        public int IdEndpoint { get; set; }
        [Required(ErrorMessage = "La acción es obligatorio")]
        public string? Accion { get; set; }
    }
}
