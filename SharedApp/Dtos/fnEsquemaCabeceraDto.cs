using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class fnEsquemaCabeceraDto
    {
        public int IdEsquema { get; set; }
        public int MostrarWebOrden { get; set; }
        public string? MostrarWeb { get; set; }
        public string? TooltipWeb { get; set; }
        public string? EsquemaVista { get; set; }
        public string? EsquemaJson { get; set; }

    }
}
