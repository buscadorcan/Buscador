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
        private List<VwFiltroDto>? listaEtiquetasFiltros = new List<VwFiltroDto>();
        private List<List<FnFiltroDetalleDto>?> listadeOpciones = new List<List<FnFiltroDetalleDto>?>();
        private List<FiltrosBusquedaSeleccion> selectedValues = new List<FiltrosBusquedaSeleccion>();
        private BuscarRequest buscarRequest = new BuscarRequest();
        private bool modoBuscar = false;
        private List<Item> ListTypeSearch = new TypeSearch().ListTypeSearch;
        private List<FnPredictWordsDto> ListFnPredictWordsDto = new List<FnPredictWordsDto>();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (iCatalogosService != null) {
                    listaEtiquetasFiltros = await iCatalogosService.GetFiltrosAsync();

                    if (listaEtiquetasFiltros != null)
                    {
                        foreach(var opciones in listaEtiquetasFiltros)
                        {
                            listadeOpciones.Add(await iCatalogosService.GetFiltroDetalleAsync<List<FnFiltroDetalleDto>>("filters/data", opciones.CodigoHomologacion));
                            
                        }
                        
                    }
                }

                // Si no hay datos del servicio, usar datos simulados
                if (listaEtiquetasFiltros == null || !listaEtiquetasFiltros.Any())
                {
                    CargarDatosSimulados();
                }
            } catch (Exception e) {
                Console.WriteLine(e);
                CargarDatosSimulados();
            }
        }

        private void CargarDatosSimulados()
        {
            // Crear etiquetas de filtro simuladas
            listaEtiquetasFiltros = new List<VwFiltroDto>
            {
                new VwFiltroDto { IdHomologacion = 1, MostrarWeb = "Filtro 1" },
                new VwFiltroDto { IdHomologacion = 2, MostrarWeb = "Filtro 2" },
                new VwFiltroDto { IdHomologacion = 3, MostrarWeb = "Filtro 3" }
            };

            // Crear opciones simuladas para cada filtro
            listadeOpciones = new List<List<FnFiltroDetalleDto>?>
            {
                new List<FnFiltroDetalleDto>
                {
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 1.1" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 1.2" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 1.3" }
                },
                new List<FnFiltroDetalleDto>
                {
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 2.1" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 2.2" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 2.3" }
                },
                new List<FnFiltroDetalleDto>
                {
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 3.1" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 3.2" },
                    new FnFiltroDetalleDto { MostrarWeb = "Opción 3.3" }
                }
            };
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
        private async Task BuscarPalabraRequest()
        {
            if (childComponentRef != null && childComponentRef.grid != null)
            {
                childComponentRef.ModoBuscar = modoBuscar;
                await childComponentRef.grid.ResetPageNumber();
            }

            await Task.CompletedTask;
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