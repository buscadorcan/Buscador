using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Models.Dtos
{
    public class VwActualizacionONADto
    {
        public string Fecha { get; set; } = "";
        public string ONA { get; set; } = "";
        public int Actualizaciones { get; set; }
    }
}
