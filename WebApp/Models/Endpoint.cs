using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class Endpoint : BaseEntity
    {
        [Key]
        public int IdEndpoint { get; set; }
        [Required]
        public string? Nombre { get; set; }
        [Required]
        public string? Url { get; set; }
    }
}