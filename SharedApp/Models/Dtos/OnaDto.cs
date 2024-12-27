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

        public string? RazonSocial { get; set; }

        public string? Siglas { get; set; }

        public string? Pais { get; set; }

        public string? Ciudad { get; set; }
        public string? Correo { get; set; }
        public string? Direccion { get; set; }
        public string? PaginaWeb { get; set; }
        public string? Telefono { get; set; }
        public string? UrlIcono { get; set; }
        public string? UrlLogo { get; set; }
        public string? InfoExtraJson { get; set; }
        public string? Estado { get; set; }
    }
}
