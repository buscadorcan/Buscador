using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;
public class CanFullText
{
    [Key]
    public int IdCanFullText   { get; set; }
    public int IdCanDataSet       { get; set; }
    public int? IdHomologacion          { get; set; }
    public string? IdEnte       { get; set; }
    public string? IdVista              { get; set; }
    public string? FullTextData { get; set; }
}