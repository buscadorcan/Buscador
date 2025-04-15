namespace SharedApp.Dtos
{
    public class UsuarioAutenticacionRespuestaDto
    {
        public string? Token { get; set; }
        public UsuarioDto? Usuario { get; set; }
        public VwRolDto? Rol { get; set; }
        public VwHomologacionGrupoDto? HomologacionGrupo { get; set; }
    }
}