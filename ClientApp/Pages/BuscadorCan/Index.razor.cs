using BlazorBootstrap;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Data;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class Index
    {
        [Inject]
        public ICatalogosService? iCatalogosService { get; set; }
        [Inject]
        public IBusquedaService? iBusquedaService { get; set; }
        private IndexGrilla? childComponentRef;
        private List<HomologacionDto>? listaEtiquetasFiltros = new List<HomologacionDto>();
        private List<List<HomologacionDto>?> listadeOpciones = new List<List<HomologacionDto>?>();
        private List<List<int>> selectedValues = new List<List<int>>();
        private BuscarRequest buscarRequest = new BuscarRequest();
        private string TextSearch = default!;
        private int ModoBuscar = 1;
        private List<Item> ListTypeSearch = new TypeSearch().ListTypeSearch;
        private List<FnPredictWordsDto> ListFnPredictWordsDto = new List<FnPredictWordsDto>();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (iCatalogosService != null) {
                    listaEtiquetasFiltros = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("etiquetas_filtro");

                    if (listaEtiquetasFiltros != null)
                    {
                        foreach(var opciones in listaEtiquetasFiltros)
                        {
                            listadeOpciones.Add(await iCatalogosService.GetHomologacionDetalleAsync<List<HomologacionDto>>("filtro_detalles", opciones.IdHomologacion));
                        }
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        private void CambiarSeleccion(int selectedValue, int index)
        {
            while (selectedValues.Count <= index)
            {
                selectedValues.Add(new List<int>());
            }

            if (selectedValues[index].Contains(selectedValue))
            {
                selectedValues[index].Remove(selectedValue);
            }
            else
            {
                selectedValues[index].Add(selectedValue);
            }
        }
        private async Task BuscarPalabraRequest(int modoBuscar)
        {
            ModoBuscar = modoBuscar;
            if (childComponentRef != null && childComponentRef.grid != null)
            {
                childComponentRef.ModoBuscar = modoBuscar;
                await childComponentRef.grid.ResetPageNumber();
            }

            await Task.CompletedTask;
        }
        private async Task BuscarPalabraRequest()
        {
            await BuscarPalabraRequest(ModoBuscar);
        }
        private async Task<AutoCompleteDataProviderResult<FnPredictWordsDto>> FnPredictWordsDtoDataProvider(AutoCompleteDataProviderRequest<FnPredictWordsDto> request)
        {
            buscarRequest.TextoBuscar = request.Filter.Value;
            if (iBusquedaService != null)
            {
                var words = await iBusquedaService.FnPredictWords(request.Filter.Value);
                return await Task.FromResult(new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = words, TotalCount = words.Count() });
            }

            return await Task.FromResult(new AutoCompleteDataProviderResult<FnPredictWordsDto> { Data = [], TotalCount = 0 });
        }
        private void OnAutoCompleteChanged(FnPredictWordsDto _fnPredictWordsDto)
        {
            buscarRequest.TextoBuscar = _fnPredictWordsDto.Word ?? "";
        }
    }
}