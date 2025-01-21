using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class GrillaCard 
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
        private List<BuscadorResultadoDataDto>? resultadoData;
        private List<VwGrillaDto>? listaEtiquetasGrilla;

        //protected override async Task OnInitializedAsync()
        //{
        //    try
        //    {
        //        // Cargar etiquetas
        //        if (iCatalogosService != null)
        //        {
        //            listaEtiquetasGrilla = await iCatalogosService.GetHomologacionAsync<List<VwGrillaDto>>("grid/schema");
        //            Console.WriteLine($"Etiquetas cargadas: {JsonConvert.SerializeObject(listaEtiquetasGrilla)}");
        //        }

        //        // Cargar datos iniciales
        //        await CargarResultados();
        //        Console.WriteLine($"Datos cargados: {JsonConvert.SerializeObject(resultadoData)}");
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error en OnInitializedAsync: {ex.Message}");
        //    }
        //}


        private async Task CargarResultados()
        {
            try
            {
                if (servicio == null) return;

                var filtros = new
                {
                    ExactaBuscar = false,
                    TextoBuscar = buscarRequest?.TextoBuscar ?? "",
                    FiltroPais = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_PAI")?.Seleccion ?? new List<string>(),
                    FiltroOna = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ONA")?.Seleccion ?? new List<string>(),
                    FiltroNorma = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_NOR")?.Seleccion ?? new List<string>(),
                    FiltroEsquema = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ESQ")?.Seleccion ?? new List<string>(),
                    FiltroEstado = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_EST")?.Seleccion ?? new List<string>(),
                    FiltroRecomocimiento = selectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_REC")?.Seleccion ?? new List<string>()
                };

                var result = await servicio.PsBuscarPalabraAsync(JsonConvert.SerializeObject(filtros), 1, 10); // Página y tamaño configurables
                resultadoData = result.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CargarResultados: {ex.Message}");
            }
        }

        private void MostrarDetalle(BuscadorResultadoDataDto item)
        {
            Console.WriteLine($"Mostrando detalle del item: {item.DataEsquemaJson}");
            // Lógica para mostrar detalles (como un modal)
        }

        private async Task AbrirPdf(BuscadorResultadoDataDto item)
        {
            try
            {
                var pdfUrl = await GetPdfUrlFromEsquema(item);
                if (string.IsNullOrEmpty(pdfUrl))
                {
                    Console.WriteLine("No se encontró la URL del PDF.");
                    return;
                }

                Console.WriteLine($"Abriendo PDF: {pdfUrl}");
                // Lógica para abrir el PDF (puede ser en un modal o redirigiendo)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al abrir el PDF: {ex.Message}");
            }
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

                return certificado?.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la URL del certificado: {ex.Message}");
                return null;
            }
        }

        public class JsonData
        {
            public int IdHomologacion { get; set; }
            public string Data { get; set; }
        }
    }
}
