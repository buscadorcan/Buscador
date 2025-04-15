using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Models
{
  public class LogScript : BaseEntity
  {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdLogScript { get; set; }
    [Required]
    public string? StateLog { get; set; }
    [Required]
    public string? TextLog { get; set; }
    [Required]
    public string? TimeRun { get; set; }
    [Required]
    public string? NameScript { get; set; }
    [Required]
    public DateTime? TimeLog { get; set; }
    [Required]
    public string? HostName { get; set; }
    [Required]
    public string? LoggedInUser { get; set; }
    [Required]
    public bool IdLogTrace { get; set; }
  }
}
