using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class FiltrosBusquedaSeleccionDto
    {
        public string CodigoHomologacion { get; set; } = string.Empty;
        public List<string> Seleccion { get; set; } = new List<string>();
    }
}
