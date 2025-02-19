using System.ComponentModel.DataAnnotations;

namespace SharedApp.Models.Dtos
{
    public class AuthValidationDto
    {
        [Required]
        public int IdUsuario { get; set; }
        [Required]
        public int IdHomologacionRol { get; set; }
        [Required]
        public string Codigo { get; set;} = "";
    }
}