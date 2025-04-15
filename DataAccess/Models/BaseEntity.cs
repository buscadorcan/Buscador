namespace DataAccess.Models
{
  public abstract class BaseEntity
  {
    public DateTime? FechaCreacion { get; set; } = DateTime.Now;
    public DateTime? FechaModifica { get; set; } = DateTime.Now;
    public int IdUserCreacion { get; set; } = 1;
    public int IdUserModifica { get; set; } = 1;
  }
}
