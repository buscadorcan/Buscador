using System.ComponentModel.DataAnnotations;

namespace SharedApp.Models.Dtos
{
  public class ConexionDto
  {
    public int IdConexion { get; set; }
    public int IdSistema { get; set; }
    [Required]
    public string? BaseDatos { get; set; }
    [Required]
    public string? Host { get; set; }
    [Required]
    public int Puerto { get; set; }
    [Required]
    public string? Usuario { get; set; }
    public string? Contrasenia { get; set; }
    [Required]
    public string? MotorBaseDatos { get; set; }
    public string? Filtros { get; set; }
    public int TiempoEspera { get; set; }
    public string? Migrar { get; set; }
  }
}