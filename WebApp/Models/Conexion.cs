using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;
public class Conexion : BaseEntity
{
    [Key]
    public int IdConexion { get; set; }
    public int IdUsuario { get; set; }
    public int IdSistema { get; set; }
    public string? BaseDatos { get; set; }
    public string? Host { get; set; }
    public int Puerto { get; set; }
    public string? Usuario { get; set; }
    public string? Contrasenia { get; set; }
    public string? MotorBaseDatos { get; set; }
    public string? Filtros { get; set; }
    public DateTime? FechaConexion { get; set; }
    public int TiempoEspera { get; set; }
}