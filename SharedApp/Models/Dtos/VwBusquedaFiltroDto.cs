using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Models.Dtos
{
    public class VwBusquedaFiltroDto
    {
        public string FiltroPor { get; set; } = "";
        public int Busqueda { get; set; }
    }
}
