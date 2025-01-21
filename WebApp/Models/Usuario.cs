using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  public class Usuario : BaseEntity
  {
    [Key]
    public int IdUsuario { get; set; }
 
    public int IdHomologacionRol { get; set; }
    public int IdONA { get; set; }

    public string? Nombre { get; set; }
   
    public string? Apellido { get; set; }
  
    public string? Telefono { get; set; }

    public string? Email { get; set; }
   
    public string? Clave { get; set; }

    public string? Estado { get; set; }
    [ForeignKey("IdHomologacionRol")]
    public Homologacion? Homologacion { get; set; }

    }
}
