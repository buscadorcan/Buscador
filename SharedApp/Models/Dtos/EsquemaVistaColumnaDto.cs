using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Models.Dtos
{
    public class EsquemaVistaColumnaDto
    {
        
        public int? IdEsquemaVistaColumna { get; set; }
        public int? IdEsquemaVista { get; set; }
        public int? ColumnaEsquemaIdH { get; set; }
        public string? ColumnaEsquema { get; set; } = "";
        public string? ColumnaVista { get; set; } = "";
        public bool ColumnaVistaPK { get; set; } 
        public string? Estado { get; set; } = "A";
    }
}
