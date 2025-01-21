using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Models.Dtos
{
    public class OnaDto
    {
        public int IdONA { get; set; }
        [Required(ErrorMessage = "El pais es obligatorio.")]
        public int? IdHomologacionPais { get; set; }
        [Required(ErrorMessage = "La Razón Social es obligatoria.")]
        public string? RazonSocial { get; set; }
        [Required(ErrorMessage = "Las Siglas son obligatorias.")]
        public string? Siglas { get; set; }
        [Required(ErrorMessage = "Las ciudad es obligatoria.")]
        public string? Ciudad { get; set; }
        [Required(ErrorMessage = "El Correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        [Url(ErrorMessage = "El formato de la URL no es válido.")]
        public string? PaginaWeb { get; set; }
        [Required(ErrorMessage = "El Teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido.")]
        public string? Telefono { get; set; }
        public string? UrlIcono { get; set; }
        public string? UrlLogo { get; set; }
        public string? InfoExtraJson { get; set; }
        public string? Estado { get; set; }
        public int? IdUserCreacion { get; set; }
        public int? IdUserModifica { get; set; }

    }
}
