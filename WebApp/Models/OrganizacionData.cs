using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
  [Table("OrganizacionData")]
  public class OrganizacionData
  {
    [Key]
    public int IdOrganizacionData		  { get; set; }
    public int IdConexion				      { get; set; }
    public int IdHomologacionEsquema  { get; set; }
    public string? IdOrganizacion     { get; set; }
    public string? IdVista            { get; set; }
    public string? DataEsquemaJson    { get; set; }
    public DateTime? FechaCreacion    { get; set; }
    public DateTime? DataFecha	      { get; set; }

  }
}