using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using ClientApp.Services.IService;
using ClientApp.Models;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class IndexGrilla
    {
        [Parameter]
        public BuscarRequest? buscarRequest { get; set; }
        [Parameter]
        public List<FiltrosBusquedaSeleccion>? selectedValues { get; set; }
        [Parameter]
        public List<HomologacionDto>? listaEtiquetasFiltros { get; set; }
        [Inject]
        public IBusquedaService? servicio { get; set; }
        [Inject]
        public ICatalogosService? iCatalogosService { get; set; }
        private Modal modal = default!;
        public Grid<BuscadorResultadoDataDto>? grid;
        private List<HomologacionDto>? listaEtiquetasGrilla;
        private int totalCount = 0;
        public int ModoBuscar { get; set;}
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (iCatalogosService != null)
                {
                    listaEtiquetasGrilla = await iCatalogosService.GetHomologacionAsync<List<HomologacionDto>>("etiquetas_grilla");
                }
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
        private async Task<List<BuscadorResultadoDataDto>> BuscarEsquemas(int PageNumber, int PageSize)
        {
            var listBuscadorResultadoDataDto = new List<BuscadorResultadoDataDto>();
            try {
                if (servicio != null)
                {
                    var result = await servicio.PsBuscarPalabraAsync(JsonConvert.SerializeObject(new {
                        ModoBuscar = ModoBuscar,
                        TextoBuscar = buscarRequest?.TextoBuscar ?? "",
                        FiltroPais = selectedValues?.FirstOrDefault( c => c.Id == 2)?.Seleccion?.Where(c => c != null ).ToList() ?? [],
                        FiltroOna = selectedValues?.FirstOrDefault( c => c.Id == 3)?.Seleccion?.Where(c => c != null ).ToList() ?? [],
                        FiltroNorma = selectedValues?.FirstOrDefault( c => c.Id == 5)?.Seleccion?.Where(c => c != null ).ToList() ?? [],
                        FiltroEsquema = selectedValues?.FirstOrDefault( c => c.Id == 4)?.Seleccion?.Where(c => c != null ).ToList() ?? [],
                        FiltroEstado = selectedValues?.FirstOrDefault( c => c.Id == 6)?.Seleccion?.Where(c => c != null ).ToList() ?? [],
                        FiltroRecomocimiento = selectedValues?.FirstOrDefault( c => c.Id == 7)?.Seleccion?.Where(c => c != null ).ToList() ?? []
                    }), PageNumber, PageSize);

                    if (!(result.Data is null)) {
                        listBuscadorResultadoDataDto = result.Data;
                    }

                    if (PageNumber == 1) {
                        totalCount = result.TotalCount;
                    }
                }
            } catch (Exception e) { 
                Console.WriteLine(e);
            }

            return listBuscadorResultadoDataDto;
        }
        private async Task<GridDataProviderResult<BuscadorResultadoDataDto>> ResultadoBusquedaDataProvider(GridDataProviderRequest<BuscadorResultadoDataDto> request)
        {
            return await Task.FromResult(new GridDataProviderResult<BuscadorResultadoDataDto> {
                Data = await BuscarEsquemas(request.PageNumber, request.PageSize),
                TotalCount = totalCount
            });
        }
        private async void showModal(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            await modal.ShowAsync<EsquemaModal>(title: "Información Detallada", parameters: parameters);
        }
    }
}