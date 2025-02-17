using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Models.Dtos
{
    public class EventTrackingDto
    {
            public string TipoUsuario { get; set; }
            public string NombreUsuario { get; set; }
            public string NombrePagina { get; set; }
            public string NombreControl { get; set; }
            public string NombreAccion { get; set; }
            public string UbicacionJson { get; set; }
            public string ParametroJson { get; set; } = "{}";

    }
}
