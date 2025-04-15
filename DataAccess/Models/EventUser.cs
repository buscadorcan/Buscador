namespace DataAccess.Models
{
    public class EventUser
    {
        public int CodigoEvento { get; set; }
        public int CodigoUsuario { get; set; }
        public string OnaSiglas { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string UsuarioEmail { get; set; }
        public string UsuarioTipo { get; set; }
        public string Pagina { get; set; }
        public string PaginaControl { get; set; }
        public string PaginaAccion { get; set; }
        public string UsuarioIP { get; set; }
        public string UsuarioPais { get; set; }
        public string UsuarioCiudad { get; set; }
        public string? ExactaBuscar { get; set; }
        public string? TextoBuscar { get; set; }
        public string? FiltroPais { get; set; }
        public string? FiltroOna { get; set; }
        public string? FiltroEsquema { get; set; }
        public string? FiltroNorma { get; set; }
        public string? FiltroEstado { get; set; }
        public string? FiltroRecomocimiento { get; set; }
        public string ErrorTracking { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
