using System.ComponentModel.DataAnnotations;

namespace SharedApp.Dtos
{
    public class MigracionExcelDto
    {
      public int IdMigracionExcel { get; set; }
      public int MigracionNumero { get; set; }
      [Required]
      public string? MigracionEstado { get; set; }
      public string? ExcelFileName { get; set; }
      public string? MensageError { get; set; }
    }
}