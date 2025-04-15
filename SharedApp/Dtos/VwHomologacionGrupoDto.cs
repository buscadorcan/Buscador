using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class VwHomologacionGrupoDto
    {
        public int IdHomologacion { get; set; }
        public int? IdHomologacionGrupo { get; set; } = 0;
        public string? MostrarWeb { get; set; } = "";
        public string? TooltipWeb { get; set; } = "";
        public int? MostrarWebOrden { get; set; } = 0;
        public string? CodigoHomologacion { get; set; } = "";
        public string? Estado { get; set; } = "";
    }
}
