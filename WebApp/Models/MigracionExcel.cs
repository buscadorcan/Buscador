using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;
public class MigracionExcel
{
    public MigracionExcel()
    {
        FechaCreacion = DateTime.Now;
        MensageError = "";
    }

    [Key]
    public int IdMigracionExcel { get; set; }
    public int MigracionNumero { get; set; }
    public string? MigracionEstado { get; set; }
    public string? ExcelFileName { get; set; }
    public string? MensageError { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public int IdUserCreacion { get; set; }
}