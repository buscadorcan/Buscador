using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class IndexCard
    {
        [Parameter]
        public BuscarRequest? buscarRequest { get; set; }
        [Parameter]
        public List<FiltrosBusquedaSeleccion>? selectedValues { get; set; }

        [Parameter]
        public List<VwFiltroDto>? listaEtiquetasFiltros { get; set; }
        [Inject]
        public IBusquedaService? servicio { get; set; }
        [Inject]
        public ICatalogosService? iCatalogosService { get; set; }
        private Modal modal = default!;
        public Grid<BuscadorResultadoDataDto>? grid;
        private List<VwGrillaDto>? listaEtiquetasGrilla;
        private List<BuscadorResultadoDataDto> resultados = new List<BuscadorResultadoDataDto>();

        private int totalCount = 0;
        public bool ModoBuscar { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Llamar al DataProvider para obtener los datos
                var result = await ResultadoBusquedaDataProvider(new GridDataProviderRequest<BuscadorResultadoDataDto>());
                if (result != null)
                {
                    resultados = result.Data.ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private async Task<List<BuscadorResultadoDataDto>> BuscarEsquemas(int PageNumber, int PageSize)
        {
            var listBuscadorResultadoDataDto = new List<BuscadorResultadoDataDto>();

            try
            {
                if (servicio != null)
                {
                    var filtros = new
                    {
                        ExactaBuscar = ModoBuscar,
                        TextoBuscar = buscarRequest?.TextoBuscar ?? "",
                        FiltroPais = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_PAI")?.Seleccion ?? new List<string>(),
                        FiltroOna = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ONA")?.Seleccion ?? new List<string>(),
                        FiltroNorma = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_NOR")?.Seleccion ?? new List<string>(),
                        FiltroEsquema = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ESQ")?.Seleccion ?? new List<string>(),
                        FiltroEstado = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_EST")?.Seleccion ?? new List<string>(),
                        FiltroRecomocimiento = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_REC")?.Seleccion ?? new List<string>()
                    };

                    Console.WriteLine($"Filtros enviados: {JsonConvert.SerializeObject(filtros)}");

                    var result = await servicio.PsBuscarPalabraAsync(JsonConvert.SerializeObject(filtros), PageNumber, PageSize);

                    if (!(result.Data is null))
                    {
                        listBuscadorResultadoDataDto = result.Data;
                    }

                    if (PageNumber == 1)
                    {
                        totalCount = result.TotalCount;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error en BuscarEsquemas: {e.Message}");
            }

            return listBuscadorResultadoDataDto;
        }
        private async Task<GridDataProviderResult<BuscadorResultadoDataDto>> ResultadoBusquedaDataProvider(GridDataProviderRequest<BuscadorResultadoDataDto> request)
        {
            return await Task.FromResult(new GridDataProviderResult<BuscadorResultadoDataDto>
            {
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
