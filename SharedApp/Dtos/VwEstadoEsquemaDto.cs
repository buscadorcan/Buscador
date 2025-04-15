using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class VwEstadoEsquemaDto
    {
        public string Esquema { get; set; } = "";
        public string Estado { get; set; } = "";
        public int Organizacion { get; set; }
    }
}
