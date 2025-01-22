using System.ComponentModel.DataAnnotations;

namespace SharedApp.Models.Dtos
{
    public class PropiedadesTablaDto
    {
        [Required]
        public string? NombreColumna { get; set; }
        public bool IsValid { get; set; }

    }
}
