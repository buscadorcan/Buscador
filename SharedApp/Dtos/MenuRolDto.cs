using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class MenuRolDto
    {
        public int IdMenuRol { get; set; } = 0;
        public int? IdHRol { get; set; } = 0;
        public string? Rol { get; set; } = "";
        public int? IdHMenu { get; set; } = 0;
        public string? Menu { get; set; } = "";
        [Required]
        public string? Estado { get; set; } = "A";
        [Required]
        public DateTime? FechaCreacion { get; set; } = DateTime.Now;

    }
}
