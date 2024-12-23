using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
  /// <summary>
  /// Componente que gestiona la selección de filtros en la búsqueda.
  /// </summary>
  public partial class FilterSelect
  {
    /// <summary>
    /// Servicio de API inyectado para realizar llamadas a endpoints.
    /// </summary>
    [Inject] 
    public IApiService _apiService { get; set; } = default!;

    /// <summary>
    /// Servicio de notificaciones de tipo Toast inyectado.
    /// </summary>
    [Inject] 
    protected ToastService ToastService { get; set; } = default!;

    /// <summary>
    /// Lista de filtros seleccionados por el usuario.
    /// </summary>
    [Parameter]
    public List<FiltroSeleccionado> selectedFilters { get; set; } = new();

    /// <summary>
    /// Lista de filtros obtenidos del esquema remoto.
    /// </summary>
    private List<VwFiltroDto>? listVwFiltroDto;

    /// <summary>
    /// Método de inicialización del componente que carga los filtros disponibles y sus detalles.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
      try
      {
        // Obtiene el esquema de filtros.
        listVwFiltroDto = await GetFilterSchemaAsync();

        // Carga los detalles de cada filtro.
        if (listVwFiltroDto.Any())
        {
          var detailTasks = listVwFiltroDto.Select(async item => {
            item.Detalles = await _apiService.GetAsync<List<FnFiltroDetalleDto>>($"filters/data/{item.IdHomologacion}");
          });

          await Task.WhenAll(detailTasks);
        }
      }
      catch (Exception e)
      {
        // Notifica errores durante la carga.
        ToastService.Notify(new(ToastType.Danger, $"Error fetching filters: {e.Message}."));
      }
    }

    /// <summary>
    /// Obtiene el esquema de filtros desde el endpoint remoto y los ordena por prioridad.
    /// </summary>
    /// <returns>Lista ordenada de <see cref="VwFiltroDto"/>.</returns>
    private async Task<List<VwFiltroDto>> GetFilterSchemaAsync()
    {
      try
      {
        var result = await _apiService.GetAsync<List<VwFiltroDto>>("filters/schema");
        return result?.OrderBy(c => c.MostrarWebOrden).ToList() ?? new List<VwFiltroDto>();
      }
      catch (Exception)
      {
        // Notifica errores durante la carga del esquema.
        ToastService.Notify(new(ToastType.Danger, "Error loading filter schema."));
          return new List<VwFiltroDto>();
      }
    }

    /// <summary>
    /// Cambia la selección de un filtro específico basado en el valor seleccionado y el ID del filtro.
    /// </summary>
    /// <param name="selectedValue">Valor seleccionado por el usuario.</param>
    /// <param name="id">Identificador único del filtro.</param>
    private void CambiarSeleccion(string selectedValue, int? id)
    {
      if (id == null) return;

      // Busca el filtro seleccionado por ID.
      var filter = selectedFilters.FirstOrDefault(c => c.Id == id);
      if (filter != null)
      {
        // Agrega o elimina el valor de la selección.
        if (filter.Seleccion?.Contains(selectedValue) == true)
        {
          filter.Seleccion?.Remove(selectedValue);
        }
        else
        {
          filter.Seleccion?.Add(selectedValue);
        }
      }
      else
      {
        // Crea un nuevo filtro si no existe en la lista de seleccionados.
        selectedFilters.Add(new FiltroSeleccionado {
          Id = id,
          Seleccion = new List<string> { selectedValue }
        });
      }
    }
  }
}
