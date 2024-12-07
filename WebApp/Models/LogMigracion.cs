using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models;
public class LogMigracion
{
    [Key]
    public int IdLogMigracion { get; set; }
    public int IdConexion { get; set; } = 0;
    public string CodigoHomologacion { get; set; } = "";
    public string Host { get; set; } = "";
    public int Puerto { get; set; } = 0;
    public string Usuario { get; set; } = "";
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public int Migracion { get; set; }
    public string Estado { get; set; } = "START";
    public string OrigenFormato { get; set; } = "EXCEL";
    public string OrigenSistema { get; set; } = "";
    public string OrigenVista { get; set; } = "";
    public int OrigenFilas { get; set; } = 0;
    public int EsquemaId { get; set; } = 0;
    public string EsquemaVista { get; set; } = "";
    public int EsquemaFilas { get; set; } = 0;
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string? Tiempo { get; set; }
    public DateTime? Inicio { get; set; } = DateTime.Now;
    public DateTime? Final { get; set; } = DateTime.Now;
    public DateTime? Fecha { get; set; } = DateTime.Now;
    [DefaultValue("")]
    public string Observacion { get; set; } = "";
}