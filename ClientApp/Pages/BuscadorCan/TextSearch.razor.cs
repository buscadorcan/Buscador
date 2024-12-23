using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
  /// <summary>
  /// Componente Blazor que permite realizar búsquedas de texto utilizando un servicio de API para predecir palabras y realizar la búsqueda.
  /// </summary>
  public partial class TextSearch
  {
    /// <summary>
    /// Servicio de API inyectado para realizar llamadas a los endpoints de la API.
    /// Este servicio se utiliza para interactuar con el backend y obtener los resultados de la búsqueda.
    /// </summary>
    [Inject] 
    public IApiService _apiService { get; set; } = default!;

    /// <summary>
    /// Servicio de notificaciones tipo Toast inyectado para mostrar mensajes al usuario.
    /// Este servicio es utilizado para mostrar notificaciones en caso de errores o información relevante.
    /// </summary>
    [Inject] 
    protected ToastService ToastService { get; set; } = default!;

    /// <summary>
    /// Parámetro de entrada que representa la solicitud de búsqueda. 
    /// Contiene el texto que se busca y los filtros aplicados.
    /// </summary>
    [Parameter] 
    public SolicitudBusqueda solicitudBusqueda { get; set; } = default!;

    /// <summary>
    /// Parámetro de salida para manejar el evento de búsqueda en el componente padre.
    /// Este callback se invoca cuando se realiza la acción de búsqueda.
    /// </summary>
    [Parameter] 
    public EventCallback HandleSearch { get; set; }

    /// <summary>
    /// Método que se invoca cuando el usuario realiza una búsqueda.
    /// Llama al evento `HandleSearch` si está delegado para realizar la acción de búsqueda.
    /// </summary>
    private async Task OnSearch()
    {
      if (HandleSearch.HasDelegate)
      {
        await HandleSearch.InvokeAsync();
      }
    }

    /// <summary>
    /// Método que proporciona los datos de predicción de palabras para autocompletar el texto de búsqueda.
    /// Utiliza el servicio de API para obtener una lista de palabras relacionadas con la entrada del usuario.
    /// </summary>
    /// <param name="request">La solicitud de autocompletado con el filtro ingresado por el usuario.</param>
    /// <returns>Un objeto <see cref="AutoCompleteDataProviderResult{FnPredictWordsDto}"/> con los datos y el total de elementos encontrados.</returns>
    private async Task<AutoCompleteDataProviderResult<FnPredictWordsDto>> FnPredictWordsDtoDataProvider(AutoCompleteDataProviderRequest<FnPredictWordsDto> request)
    {
      try
      {
        solicitudBusqueda.TextoBusqueda = request.Filter.Value;
        var words = await _apiService.GetAsync<List<FnPredictWordsDto>>($"predictWords?word={request.Filter.Value}");
        return await Task.FromResult(new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = words, TotalCount = words.Count() });
      }
      catch (Exception e)
      {
        // Notifica errores durante la carga de predicciones de palabras.
        ToastService.Notify(new(ToastType.Danger, $"Error fetching filters: {e.Message}."));
      }

      return await Task.FromResult(new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = [], TotalCount = 0 });
    }

    /// <summary>
    /// Método que se invoca cuando se selecciona una palabra del autocompletado.
    /// Actualiza el texto de búsqueda o limpia los filtros si no se selecciona ninguna palabra.
    /// </summary>
    /// <param name="_fnPredictWordsDto">El objeto de predicción de palabra seleccionado.</param>
    private void OnAutoCompleteChanged(FnPredictWordsDto _fnPredictWordsDto)
    {
      if (_fnPredictWordsDto?.Word != null)
      {
        solicitudBusqueda.TextoBusqueda = _fnPredictWordsDto.Word;
      }
      else
      {
        solicitudBusqueda.Filtros = new List<FiltroSeleccionado>();
      }
    }
  }
}
