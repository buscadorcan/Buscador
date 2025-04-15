namespace SharedApp.Dtos
{
    public class paAddEventTrackingDto
    {
        public string CodigoHomologacionRol { get; set; } = string.Empty;

        public string NombreUsuario { get; set; } = string.Empty;

        public string CodigoHomologacionMenu { get; set; } = string.Empty;

        public string NombreControl { get; set; } = string.Empty;

        public string NombreAccion { get; set; } = string.Empty;
        public string UbicacionJson { get; set; } = "{}";

        public string ParametroJson { get; set; } = "{}";

        public string ErrorTracking { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    }
}
