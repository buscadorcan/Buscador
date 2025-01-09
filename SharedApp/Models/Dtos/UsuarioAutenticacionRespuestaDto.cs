namespace SharedApp.Models.Dtos
{
    public class UsuarioAutenticacionRespuestaDto
    {
        public string? Token { get; set; }
        public UsuarioDto? Usuario { get; set; }
        public VwRolDto? Rol { get; set; }
    }
}