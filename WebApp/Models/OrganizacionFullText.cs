using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;
public class OrganizacionFullText
{
    [Key]
    public int IdOrganizacionFullText { get; set; }
    public int IdDataLakeOrganizacion { get; set; }
    public string? IdOrganizacion { get; set; }
    public string? IdVista { get; set; }
    public int? IdHomologacion { get; set; }
    public string? FullTextOrganizacion { get; set; }
}