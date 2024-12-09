using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;
public class LogMigracionDetalle
{
    public LogMigracionDetalle() { SetDefaults(); }
    public LogMigracionDetalle(LogMigracion logMigracion)
    {
      IdLogMigracion = logMigracion.IdLogMigracion;
      NroMigracion = logMigracion.Migracion;
      EsquemaId = logMigracion.EsquemaId;
      EsquemaVista = logMigracion.EsquemaVista;
      EsquemaIdHomologacion = logMigracion.CodigoHomologacion;
      SetDefaults();
    }

    [Key]
    public int IdLogMigracionDetalle { get; set; }
    public int IdLogMigracion { get; set; }
    public int NroMigracion { get; set; }
    public int EsquemaId { get; set; }
    public string EsquemaVista { get; set; }
    public string EsquemaIdHomologacion { get; set; }
    public int IdHomologacion { get; set; }
    public string NombreHomologacion { get; set; }
    public string OrigenVistaColumna { get; set; }
    public DateTime Fecha { get; set; }

    private void SetDefaults() {
      EsquemaId = 0;
      EsquemaVista = "";
      EsquemaIdHomologacion = "";
      IdHomologacion = 0;
      NombreHomologacion = "";
      OrigenVistaColumna = "";
      Fecha = DateTime.Now;
    }
}