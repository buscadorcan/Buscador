using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
  public class EsquemaFullText
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEsquemaFullText { get; set; }
    [Required]
    public int IdEsquemaData  { get; set; }
    [Required]
    public int IdHomologacion { get; set; }
    public string? FullTextData { get; set; }
  }
}
