using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Dtos
{
    public class VwAcreditacionOADto
    {
        public string? Pais { get; set; }
        public string? ONA { get; set; }
        public int? Organizaciones { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
