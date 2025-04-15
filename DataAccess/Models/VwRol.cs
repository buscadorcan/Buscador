using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
  [Table("vwRol")]
  public class VwRol
  {
    [Key]
    public int IdHomologacionRol { get; set; }
    public string? Rol { get; set; }
    public string? CodigoHomologacion { get; set; }
    }
}
