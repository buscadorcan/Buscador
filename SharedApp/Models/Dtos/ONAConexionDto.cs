using System.ComponentModel.DataAnnotations;

namespace SharedApp.Models.Dtos
{
  public class ONAConexionDto
  {
    public int IdONA { get; set; }
    [Required]
    public string? Host { get; set; }
    [Required]
    public int Puerto { get; set; }
    [Required]
    public string? Usuario { get; set; }
    public string? Contrasenia { get; set; }
    [Required]
    public string? BaseDatos { get; set; }
    [Required]
    public string? OrigenDatos { get; set; }
    [Required]
    public string? Migrar { get; set; }
    [Required]
    public string? Estado { get; set; }
  }
}