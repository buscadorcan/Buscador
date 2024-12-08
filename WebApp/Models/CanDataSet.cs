using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  [Table("CanDataSet")]
  public class CanDataSet
  {
    [Key]
    public int IdCanDataSet		  { get; set; }
    public int? IdConexion				      { get; set; }
    public int IdHomologacionEsquema  { get; set; }
    public string? IdEnte     { get; set; }
    public string? IdVista            { get; set; }
    public string? DataEsquemaJson    { get; set; }
  }
}