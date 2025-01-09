using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Models.Dtos
{
    public class VwHomologacionGrupoDto
    {
        public int IdHomologacion { get; set; }
        public int? IdHomologacionGrupo { get; set; }
        public string? MostrarWeb { get; set; }
        public string? TooltipWeb { get; set; }
        public int? MostrarWebOrden { get; set; }
        public string? CodigoHomologacion { get; set; }
        public string? Estado { get; set; }
    }
}
