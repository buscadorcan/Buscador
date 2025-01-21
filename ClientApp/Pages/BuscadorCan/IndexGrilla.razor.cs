using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using ClientApp.Services.IService;
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
        public List<VwFiltroDto>? listaEtiquetasFiltros { get; set; }
        [Inject]
        public IBusquedaService? servicio { get; set; }
        [Inject]
        public ICatalogosService? iCatalogosService { get; set; }
        [Inject]
        public IHomologacionService? iHomologacionService { get; set; }
        private Modal modal = default!;
        private bool isDialogOpen = false; // Control de estado del diálogo
        private string? PdfUrl; // URL del PDF

        public Grid<BuscadorResultadoDataDto>? grid;
        private List<VwGrillaDto>? listaEtiquetasGrilla;
        private int totalCount = 0;
        public bool ModoBuscar { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (iCatalogosService != null)
                {
                    listaEtiquetasGrilla = await iCatalogosService.GetHomologacionAsync<List<VwGrillaDto>>("grid/schema");
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
                    Console.WriteLine($"Filtros enviados: {JsonConvert.SerializeObject(filtros)}");

                    if (!(result.Data is null))
                    {
                        listBuscadorResultadoDataDto = result.Data;
                        Console.WriteLine($"Filtros enviados: {JsonConvert.SerializeObject(filtros)}");
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

        private async Task ShowPdfDialog(BuscadorResultadoDataDto resultData)
        {
            // Obtener la URL del certificado
            var pdfUrl = GetPdfUrlFromEsquema(resultData);

            if (pdfUrl == null)
            {
                // Mostrar una alerta o manejar el error si no hay URL
                Console.WriteLine("No se encontró la URL del certificado.");
                pdfUrl = Task.FromResult("No se encontró la URL del certificado.");
            }

            // Configurar los parámetros del modal
            var parameters = new Dictionary<string, object>
            {
                { "PdfUrl", pdfUrl } // Enviar la URL al modal
            };

            // Mostrar el modal con el componente PDFModal
            await modal.ShowAsync<PdfModal>(title: "Visualizador de PDF", parameters: parameters);
        }

        private async Task<string?> GetPdfUrlFromEsquema(BuscadorResultadoDataDto resultData)
        {
            try
            {
                if (resultData.DataEsquemaJson == null || !resultData.DataEsquemaJson.Any())
                    return null;

                var homologaciones = await iHomologacionService.GetHomologacionsAsync();
                var idHomologacion = homologaciones.FirstOrDefault(x => x.NombreHomologado == "UrlCertificado")?.IdHomologacion;

                if (idHomologacion == null)
                    return null;

                var certificado = resultData.DataEsquemaJson
                    .Select(item =>
                    {
                        try
                        {
                            return System.Text.Json.JsonSerializer.Deserialize<JsonData>(item.Data);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deserializando item.Data: {item.Data}, Error: {ex.Message}");
                            return null;
                        }
                    })
                    .FirstOrDefault(data => data != null && data.IdHomologacion == idHomologacion);

                if (certificado == null || string.IsNullOrEmpty(certificado.Data))
                    return null;

                return certificado.Data;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la URL del certificado", ex);
            }
        }


        // Clase para deserializar
        public class JsonData
        {
            public int IdHomologacion { get; set; }
            public string Data { get; set; }
        }
    }
}