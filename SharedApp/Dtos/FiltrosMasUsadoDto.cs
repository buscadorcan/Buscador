using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class FiltrosMasUsadoDto
    {
        public string CodigoHomologacionRol { get; set; }
        public string IpAddress { get; set; }
        public string FiltroTipo { get; set; }
        public string FiltroValor { get; set; }
        public int Uso { get; set; }
        public double? Latitud { get; set; } = null;
        public double? Longitud { get; set; } = null;

    }
}
