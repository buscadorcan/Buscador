using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class EsquemaVistaValidacionDto
    {
        public int? IdEsquemaVista { get; set; }
        public int IdOna { get; set; }
        public int IdEsquema { get; set; }
        public string? VistaOrigen { get; set; }
        public string? Estado { get; set; }
    }
}
