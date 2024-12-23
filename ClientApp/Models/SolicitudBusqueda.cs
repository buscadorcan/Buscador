namespace ClientApp.Models
{
  /// <summary>
  /// Representa los parámetros de búsqueda para una operación de búsqueda.
  /// Esta clase contiene el texto a buscar y los filtros aplicados a la búsqueda.
  /// </summary>
  public class SolicitudBusqueda
  {
    /// <summary>
    /// Obtiene o establece el texto que se desea buscar.
    /// Este campo se utiliza para realizar la búsqueda en los registros o datos.
    /// Puede ser nulo si no se proporciona texto de búsqueda.
    /// </summary>
    public string? TextoBusqueda { get; set; }

    /// <summary>
    /// Obtiene o establece un valor que indica si la búsqueda debe realizarse utilizando una coincidencia exacta.
    /// Si es <c>true</c>, se realizará una búsqueda exacta. Si es <c>false</c>, la búsqueda será más flexible.
    /// El valor predeterminado es <c>false</c>.
    /// </summary>
    public bool UseExactMatch { get; set; } = false;

    /// <summary>
    /// Obtiene o establece los filtros seleccionados que se aplicarán durante la búsqueda.
    /// Cada filtro seleccionado está representado por una instancia de la clase <see cref="FiltroSeleccionado"/>.
    /// Los filtros permiten refinar los resultados de la búsqueda según criterios específicos.
    /// </summary>
    public List<FiltroSeleccionado> Filtros { get; set; } = default!;
  }
}
