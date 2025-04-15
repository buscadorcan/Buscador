using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
  public class MigracionExcel
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdMigracionExcel { get; set; }

    public int? MigracionNumero { get; set; }
    
    public string? MigracionEstado { get; set; }
    
    public string? ExcelFileName { get; set; }
    
    public string? MensageError { get; set; } = "";
    public DateTime? FechaCreacion { get; set; } = DateTime.Now;
    
    public int IdUserCreacion { get; set; } = 0;
  }
}
