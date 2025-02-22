using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedApp.Models.Dtos
{
    public class IpLocationDto
    {
        public string Country { get; set; }
        public string City { get; set; }
        public string Isp { get; set; }
    }
}
