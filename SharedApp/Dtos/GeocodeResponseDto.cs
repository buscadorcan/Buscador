using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class GeocodeResponseDto
    {
        public string Status { get; set; } = string.Empty; // Estado de la respuesta (OK, ZERO_RESULTS, etc.)
        public GeocodeResult[] Results { get; set; } = Array.Empty<GeocodeResult>();

        public class GeocodeResult
        {
            public Geometry Geometry { get; set; } = new Geometry();
            public string FormattedAddress { get; set; } = string.Empty; // Dirección formateada
        }

        public class Geometry
        {
            public Location Location { get; set; } = new Location();
        }

        public class Location
        {
            public double Lat { get; set; } // Latitud
            public double Lng { get; set; } // Longitud
        }
    }
}
