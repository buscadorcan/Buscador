using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
  public class UsuarioEndpoint : BaseEntity
  {
    [Key]
    public int IdUsuarioEndPoint { get; set; }
    [Required]
    public int IdHomologacionEndPoint { get; set; }
    [Required]
    public int IdUsuario { get; set; }
    [Required]
    public string? Accion { get; set; }
    [Required]
    public string Estado { get; set; } = "A";
    [ForeignKey("IdUsuario")]
    public Usuario? Usuario { get; set; }
    [ForeignKey("IdHomologacionEndPoint")]
    public Homologacion? Homologacion { get; set; }
  }
}
