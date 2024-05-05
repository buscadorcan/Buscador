namespace WebApp.Models.Dtos
{
    public class BuscadorDto
    {
        public int IdOrganizacion { get; set; } 
        public string? CodigoAcreditacion { get; set; }
        public string? RazonSocial { get; set; }
        public string? AreaAcreditacion { get; set; }
        public string? Actividad { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
    }
}