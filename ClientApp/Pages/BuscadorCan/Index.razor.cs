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
        private List<List<FnFiltroDetalleDto>?> listadeOpciones = new List<List<FnFiltroDetalleDto>?>();
        private List<FiltrosBusquedaSeleccion> selectedValues = new List<FiltrosBusquedaSeleccion>();
        private BuscarRequest buscarRequest = new BuscarRequest();
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
                            listadeOpciones.Add(await iCatalogosService.GetHomologacionDetalleAsync<List<FnFiltroDetalleDto>>("filtro_detalles", opciones.IdHomologacion));
                        }
                    }
                }
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        private void CambiarSeleccion(string selectedValue, int? id)
        {
            if (id != null)
            {
                var sValue = selectedValues.FirstOrDefault(c => c.Id == id);
                if (sValue != null) {
                    if (sValue?.Seleccion?.Contains(selectedValue) ?? false)
                    {
                        sValue.Seleccion?.Remove(selectedValue);
                    }
                    else
                    {
                        sValue?.Seleccion?.Add(selectedValue);
                    }
                } else {
                    selectedValues.Add(new FiltrosBusquedaSeleccion{
                        Id = id,
                        Seleccion = new List<string>(){ selectedValue }
                    });
                }
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
            if (_fnPredictWordsDto?.Word != null)
            {
                buscarRequest.TextoBuscar = _fnPredictWordsDto.Word;
            } else {
                selectedValues = new List<FiltrosBusquedaSeleccion>();
            }
        }
    }

    public class FiltrosBusquedaSeleccion {
        public int? Id { get; set; }
        public List<string>? Seleccion { get; set; }
        public FiltrosBusquedaSeleccion() {
            Seleccion = new List<string>();
        }
    }
}