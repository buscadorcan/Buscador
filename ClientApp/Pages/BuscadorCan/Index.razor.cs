// using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
// using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class Index
    {
        [Inject]
        public IBusquedaService iBusquedaService { get; set; } = default!;
        private ResultGrid childComponentRef = default!;
        // private List<FiltrosBusquedaSeleccion> selectedFilters { get; set; } = default!;
        private SolicitudBusqueda solicitudBusqueda = new();
        // private bool useExactMatch = false;

        /// <summary>
        /// Maneja la solicitud de búsqueda y reinicia el número de página de la cuadrícula del componente hijo si corresponde.
        /// </summary>
        private async Task HandleSearchRequestAsync()
        {
            if (childComponentRef?.grid != null)
            {
                // Configura el modo de búsqueda en el componente hijo.
                // childComponentRef.useExactMatch = useExactMatch;

                // Reinicia el número de página en la cuadrícula.
                await childComponentRef.grid.ResetPageNumber();
            }
        }

        // // private List<Item> ListTypeSearch = new TypeSearch().ListTypeSearch;
        // // private List<FnPredictWordsDto> ListFnPredictWordsDto = new List<FnPredictWordsDto>();
        // private async Task BuscarPalabraRequest()
        // {
        //     if (childComponentRef != null && childComponentRef.grid != null)
        //     {
        //         childComponentRef.ModoBuscar = modoBuscar;
        //         await childComponentRef.grid.ResetPageNumber();
        //     }

        //     await Task.CompletedTask;
        // }
        // private async Task<AutoCompleteDataProviderResult<FnPredictWordsDto>> FnPredictWordsDtoDataProvider(AutoCompleteDataProviderRequest<FnPredictWordsDto> request)
        // {
        //     buscarRequest.TextoBuscar = request.Filter.Value;
        //     if (iBusquedaService != null)
        //     {
        //         var words = await iBusquedaService.FnPredictWords(request.Filter.Value);
        //         return await Task.FromResult(new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = words, TotalCount = words.Count() });
        //     }

        //     return await Task.FromResult(new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = [], TotalCount = 0 });
        // }
        // private void OnAutoCompleteChanged(FnPredictWordsDto _fnPredictWordsDto)
        // {
        //     if (_fnPredictWordsDto?.Word != null)
        //     {
        //         buscarRequest.TextoBuscar = _fnPredictWordsDto.Word;
        //     } else {
        //         selectedFilters = new List<FiltrosBusquedaSeleccion>();
        //     }
        // }
    }
}