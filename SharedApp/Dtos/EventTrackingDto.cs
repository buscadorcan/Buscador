using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class EventTrackingDto
    {
            public int idUsuario {  get; set; }
            public int CodigoEvento { get; set; }
            public string CodigoHomologacionRol { get; set; }
            public string NombreUsuario { get; set; }
            public string CodigoHomologacionMenu { get; set; }
            public string NombreControl { get; set; }
            public string NombreAccion { get; set; }
            public string UbicacionJson { get; set; }
            public string ParametroJson { get; set; } = "{}";


    }
}
