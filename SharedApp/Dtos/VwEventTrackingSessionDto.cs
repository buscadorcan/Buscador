namespace SharedApp.Dtos
{
   public  class VwEventTrackingSessionDto
    {
        public string CodigoHomologacionRol { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TiempoDeConeccionEnMin { get; set; }
        public string? IpDirec { get; set; }
        public double? Latitud { get; set; } = null;
        public double? Longitud { get; set; } = null;
    }
}
