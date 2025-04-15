using System.ComponentModel.DataAnnotations;

namespace SharedApp.Dtos
{
    public class EndpointRegistroDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "El url es obligatorio")]
        public string? Url { get; set; }
    }
}
