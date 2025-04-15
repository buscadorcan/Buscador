using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class EsquemaVistaOnaDto
    {
        public int IdEsquemaVista { get; set; }
        public int IdEsquema { get; set; }
        public string? EsquemaVista { get; set; }
        public string? VistaOrigen { get; set; }
        public string? MostrarWeb { get; set; }
    }
}
