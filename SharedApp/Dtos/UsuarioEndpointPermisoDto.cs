namespace SharedApp.Dtos
{
    public class UsuarioEndpointPermisoDto
    {
        public int IdUsuarioEndpointPermiso { get; set; } 
        public string? IdUsuario { get; set; }
        public string? IdEndpoint { get; set; }
        public string? Accion { get; set; }
    }
}
