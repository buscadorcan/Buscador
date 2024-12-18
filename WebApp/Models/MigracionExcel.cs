using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  public class MigracionExcel
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdMigracionExcel { get; set; }
    [Required]
    public int MigracionNumero { get; set; }
    [Required]
    public string? MigracionEstado { get; set; }
    [Required]
    public string? ExcelFileName { get; set; }
    [Required]
    public string? MensageError { get; set; } = "";
    public DateTime? FechaCreacion { get; set; } = DateTime.Now;
    [Required]
    public int IdUserCreacion { get; set; } = 0;
  }
}
