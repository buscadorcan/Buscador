namespace ClientApp.Models
{
  public class FiltroSeleccionado {
    public int? Id { get; set; }
    public List<string>? Seleccion { get; set; }
    public FiltroSeleccionado() {
      Seleccion = new List<string>();
    }
  }
}
