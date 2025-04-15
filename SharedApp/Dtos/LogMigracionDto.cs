using System.ComponentModel.DataAnnotations;

namespace SharedApp.Dtos;
public class LogMigracionDto
{
    public int IdLogMigracion { get; set; }
    public int IdConexion { get; set; } = 0;
    public string CodigoHomologacion { get; set; } = "";
    public string Host { get; set; } = "";
    public int Puerto { get; set; } = 0;
    public string Usuario { get; set; } = "";
    public int Migracion { get; set; } = 0;
    public string Estado { get; set; } = "START";
    public string OrigenFormato { get; set; } = "EXCEL";
    public string OrigenSistema { get; set; } = "";
    public string OrigenVista { get; set; } = "";
    public int OrigenFilas { get; set; } = 0;
    public int EsquemaId { get; set; } = 0;
    public string EsquemaVista { get; set; } = "";
    public int EsquemaFilas { get; set; } = 0;
    public string? Tiempo { get; set; } = "";
    public DateTime? Inicio { get; set; } = DateTime.Now;
    public DateTime? Final { get; set; } = DateTime.Now;
    public DateTime? Fecha { get; set; } = DateTime.Now;
    public string Observacion { get; set; } = "";
    public string ExcelFileName { get; set; } = "";

}