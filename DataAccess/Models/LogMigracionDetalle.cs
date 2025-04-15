using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models
{
  public class LogMigracionDetalle
  {
    public LogMigracionDetalle() { SetDefaults(); }
    public LogMigracionDetalle(LogMigracion logMigracion)
    {
      // IdLogMigracion = logMigracion.IdLogMigracion;
      // NroMigracion = logMigracion.Migracion;
      // EsquemaId = logMigracion.EsquemaId;
      // EsquemaVista = logMigracion.EsquemaVista;
      // EsquemaIdHomologacion = logMigracion.CodigoHomologacion;
      SetDefaults();
    }

    [Key]
    public int IdLogMigracionDetalle { get; set; }
    public int? IdLogMigracion { get; set; }
    public int? NroMigracion { get; set; }
    public int? IdEsquemaVista { get; set; }
    public int? ColumnaEsquemaIdH { get; set; }
    public string? ColumnaEsquema { get; set; }
    public string? ColumnaVista { get; set; }
    public bool ColumnaVistaPK { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;

    private void SetDefaults() {
      // EsquemaId = 0;
      // EsquemaVista = "";
      // EsquemaIdHomologacion = "";
      // IdHomologacion = 0;
      // NombreHomologacion = "";
      // OrigenVistaColumna = "";
      // Fecha = DateTime.Now;
    }
  }
}