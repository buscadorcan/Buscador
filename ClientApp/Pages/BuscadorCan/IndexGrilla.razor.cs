using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using System.Drawing;

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
        [Inject]
        public IONAService? iOnaService { get; set; }
        private Modal modal = default!;
        private bool isDialogOpen = false; // Control de estado del diálogo
        private string? PdfUrl; // URL del PDF
        private OnaDto? OnaDto;
        public Grid<BuscadorResultadoDataDto>? grid;
        private List<VwGrillaDto>? listaEtiquetasGrilla;
        private int totalCount = 0;
        public bool ModoBuscar { get; set; }
        private bool isLoading = true;
        private Dictionary<int, string> iconUrls = new();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (iCatalogosService != null)
                {
                    listaEtiquetasGrilla = await iCatalogosService.GetHomologacionAsync<List<VwGrillaDto>>("grid/schema");
                    Console.WriteLine($"Filtros enviados");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                isLoading = false; // Marca que los datos están listos
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

                    var result = await servicio.PsBuscarPalabraAsync(JsonConvert.SerializeObject(filtros), PageNumber, PageSize);

                    if (!(result.Data is null))
                    {
                        listBuscadorResultadoDataDto = result.Data;

                        // Prepara las URLs de los íconos
                        foreach (var item in listBuscadorResultadoDataDto)
                        {
                            if (item.IdONA.HasValue && !iconUrls.ContainsKey(item.IdONA.Value))
                            {
                                iconUrls[item.IdONA.Value] = await getIconUrl(item);
                            }
                        }

                        await grid.RefreshDataAsync();
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
            try
            {
                var data = await BuscarEsquemas(request.PageNumber, request.PageSize);
                Console.WriteLine("No se encontraron resultados en BuscarEsquemas.");

                

                if (data == null || !data.Any())
                {
                    Console.WriteLine("No se encontraron resultados en BuscarEsquemas.");
                }

                if (grid != null)
                {
                    await grid.RefreshDataAsync();
                }

                return new GridDataProviderResult<BuscadorResultadoDataDto>
                {
                    Data = data,
                    TotalCount = totalCount
                };
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
        private async void showModal(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Size = ModalSize.ExtraLarge;
            await modal.ShowAsync<EsquemaModal>(title: "Información Detallada", parameters: parameters);
        }

        private async void showModalOna(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Size = ModalSize.Regular;
            await modal.ShowAsync<OnaModal>(title: "Información Organizacion", parameters: parameters);
        }


        private async Task ShowPdfDialog(BuscadorResultadoDataDto resultData)
        {
            // Obtener la URL del certificado
            var pdfUrl = await GetPdfUrlFromEsquema(resultData);
            Console.WriteLine("No se encontró la URL del certificado.");
            if (pdfUrl == null)
            {
                // Mostrar una alerta o manejar el error si no hay URL
                Console.WriteLine("No se encontró la URL del certificado.");
                pdfUrl = "No se encontró la URL del certificado.";
            }

            // Configurar los parámetros del modal
            var parameters = new Dictionary<string, object>
            {
                { "PdfUrl", pdfUrl } // Enviar la URL al modal
            };

            // Mostrar el modal con el componente PDFModal
            modal.Size = ModalSize.Large;
            await modal.ShowAsync<PdfModal>(title: "Visualizador de PDF", parameters: parameters);
        }

        private async Task<string?> GetPdfUrlFromEsquema(BuscadorResultadoDataDto resultData)
        {
            try
            {
                if (resultData.DataEsquemaJson == null || !resultData.DataEsquemaJson.Any())
                    return null;

                var homologaciones = await iHomologacionService.GetHomologacionsAsync();
                var idHomologacion = homologaciones.FirstOrDefault(x => x.CodigoHomologacion == "KEY_ESQ_CERT")?.IdHomologacion;

                if (idHomologacion == null)
                    return null;

                var urlPdf = resultData.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == idHomologacion)?.Data;

                return urlPdf;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la URL del certificado", ex);
            }
        }

        private async Task<string> getIconUrl(BuscadorResultadoDataDto resultData)
        {
            try
            {
                var idOna = resultData.IdONA;

                OnaDto = await iOnaService?.GetONAsAsync(idOna ?? 0);

                if (!string.IsNullOrEmpty(OnaDto.UrlIcono))
                {
                    var deserialized = JsonConvert.DeserializeObject<Dictionary<string, string>>(OnaDto.UrlIcono);

                    if (deserialized != null && deserialized.ContainsKey("filePath"))
                    {
                        // Si el JSON tiene "filePath", lo retornamos
                        return deserialized["filePath"];
                    }
                }

                // Retorna un ícono predeterminado si no hay URL válida
                return "https://via.placeholder.com/16";
            }
            catch (Exception ex)
            {
                // Maneja el error y retorna un ícono predeterminado
                Console.WriteLine(ex.Message);
                return "https://via.placeholder.com/16";
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