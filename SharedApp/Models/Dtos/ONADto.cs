using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Models.Dtos
{
    public class ONADto
    {
        public int IdONA { get; set; }
        [Required]
        public string? RazonSocial { get; set; }
        [Required]
        public string? Siglas { get; set; }
        [Required]
        public string? Pais { get; set; }
        [Required]
        public string? Ciudad { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public string? PaginaWeb { get; set; }
        public string? Telefono { get; set; }
        public string? UrlIcono { get; set; }
        public string? UrlLogo { get; set; }
        [Required]
        public string? InfoExtraJson { get; set; }
        [Required]
        public string? Estado { get; set; }
    }
}
