using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
    [Table("vwAcreditacionOA")]
    public class VwAcreditacionOA
    {
        //[Key]
        public string? Pais { get; set; }
        public string? ONA { get; set; }
        public int? Organizaciones { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
