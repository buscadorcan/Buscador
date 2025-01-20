using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  public class LogMigracion
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdLogMigracion { get; set; }
    [Required]
    public int IdONA { get; set; }
    [Required]
    public string? Host { get; set; } = "";
    [Required]
    public int Puerto { get; set; } = 0;
    [Required]
    public string? Usuario { get; set; } = "";
    [Required]
    public string? BaseDatos { get; set; } = "";
    [Required]
    public string? OrigenDatos { get; set; } = "EXCEL";
    [Required]
    public string? Migrar { get; set; } = "S";
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public int Migracion { get; set; } = 0;
    [Required]
    public string? Estado { get; set; } = "START";
    [Required]
    public int EsquemaId { get; set; } = 0;
    [Required]
    public string? EsquemaVista { get; set; } = "";
    [Required]
    public int EsquemaFilas { get; set; } = 0;
    [Required]
    public string VistaOrigen { get; set; } = "";
    [Required]
    public int VistaFilas { get; set; } = 0;
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string? Tiempo { get; set; } = "";
    public DateTime? Inicio { get; set; } = DateTime.Now;
    public DateTime? Final { get; set; } = DateTime.Now;
    public DateTime? Fecha { get; set; } = DateTime.Now;
    public string Observacion { get; set; } = "";
    public string? ExcelFileName { get; set; } = "";
    }
}
