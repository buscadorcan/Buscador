
namespace SharedApp.Models.Dtos
{
   public  class VwEventTrackingSessionDto
    {
        public string CodigoHomologacionRol { get; set; }
        public string NombreControl { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TiempoEnSegundos { get; set; }
        public double? Latitud { get; set; } = null;
        public double? Longitud { get; set; } = null;
        public string? IpDirec { get; set; }
    }
}
