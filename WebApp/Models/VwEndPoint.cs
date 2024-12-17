using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  [Table("vwEndPoint")]
  public class VwEndPoint
  {
    [Key]
    public int IdHomologacionEndPoint { get; set; }
    public string? EndPointNombre { get; set; }
    public string? EndPointUrl { get; set; }
  }
}
