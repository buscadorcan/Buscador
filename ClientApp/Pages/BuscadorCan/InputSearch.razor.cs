using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para el buscador de CAN.
    /// </summary>
    public partial class InputSearch : ComponentBase
    {
        /// <summary>
        /// Servicio de solicitud de búsqueda.
        /// </summary>
        [Inject] public IBusquedaService? iBusquedaService { get; set; }

        /// <summary>
        /// Evento que se dispara para cambiar el valor del input de búsqueda.
        /// </summary>
        [Parameter] public EventCallback<(string, bool)> onClickSearch { get; set; }

        /// <summary>
        /// valor del checkbox de búsqueda exacta.
        /// </summary>
        private bool isExactSearch = false;

        /// <summary>
        /// Texto del input de búsqueda.
        /// </summary>
        private string searchText = string.Empty;

        /// <summary>
        /// Temporizador de retardo para la búsqueda de texto predictivo.
        /// </summary>
        private Timer? _debounceTimer;

        /// <summary>
        /// Objeto de resultado de búsqueda de texto predictivo.
        /// </summary>
        private List<FnPredictWordsDto> ListFnPredictWordsDto = new List<FnPredictWordsDto>();

        /// <summary>
        /// Evento que se dispara cuando se cambia el checkbox.
        /// </summary>
        /// <param name="e"></param>
        private void HandleChangeExactSearch(ChangeEventArgs e)
        {
            Console.WriteLine(e.Value);
            if (e.Value is bool value)
            {
                isExactSearch = value;
            }
        }

        /// <summary>
        /// Evento que se dispara cuando se cambia el texto del input.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private void HandleChangeTextSearch(ChangeEventArgs e)
        {
            searchText = e.Value?.ToString();

            _debounceTimer?.Dispose();

            _debounceTimer = new Timer(async _ =>
            {
                await InvokeAsync(async() => await HandleSearch());
            }, null, 500, Timeout.Infinite);
        }

        /// <summary>
        /// Método que maneja la búsqueda de texto predictivo.
        /// </summary>
        /// <returns></returns>
        private async Task HandleSearch()
        {
            if (!string.IsNullOrWhiteSpace(searchText) && iBusquedaService != null)
            {
                // Crear un FilterItem con los parámetros requeridos
                var filterItem = new FilterItem(
                    propertyName: "Word",                  // Nombre de la propiedad a filtrar
                    value: searchText,                     // Valor de búsqueda
                    @operator: FilterOperator.Contains,    // Operador de comparación
                    stringComparison: StringComparison.OrdinalIgnoreCase // Tipo de comparación
                );

                // Crear la solicitud de autocompletado con el filtro
                var request = new AutoCompleteDataProviderRequest<FnPredictWordsDto>
                {
                    Filter = filterItem
                };

                ListFnPredictWordsDto = await iBusquedaService.FnPredictWords(request.Filter.Value);
                await InvokeAsync(() => StateHasChanged());
            }
            else
            {
                ListFnPredictWordsDto.Clear();
            }
        }

        /// <summary>
        /// Método que maneja el evento de clic en el botón de búsqueda.
        /// </summary>
        /// <returns></returns>
        private async Task onClickFilter() {
            await onClickSearch.InvokeAsync((searchText, isExactSearch));
        }
    }
}
