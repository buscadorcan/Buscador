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
        public BuscarRequest? BuscarRequest { get; set; } = new BuscarRequest();

        [Parameter]
        public List<FiltrosBusquedaSeleccion>? SelectedValues { get; set; } = new List<FiltrosBusquedaSeleccion>();

        [Parameter]
        public List<VwFiltroDto>? ListaEtiquetasFiltros { get; set; } = new List<VwFiltroDto>();

        [Inject]
        public IBusquedaService? Servicio { get; set; }

        [Inject]
        public ICatalogosService? CatalogosService { get; set; }

        [Inject]
        public IHomologacionService? HomologacionService { get; set; }

        // Propiedades para la vista
        public List<BuscadorResultadoDataDto>? ResultadoData { get; private set; } = new List<BuscadorResultadoDataDto>();
        public List<VwGrillaDto>? ListaEtiquetasGrilla { get; private set; } = new List<VwGrillaDto>();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Cargar etiquetas
                if (CatalogosService != null)
                {
                    ListaEtiquetasGrilla = await CatalogosService.GetHomologacionAsync<List<VwGrillaDto>>("grid/schema")
                                         ?? new List<VwGrillaDto>();
                }

                // Cargar resultados iniciales
                await CargarResultados();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en OnInitializedAsync: {ex.Message}");
            }
        }

        private async Task CargarResultados()
        {
            try
            {
                if (Servicio == null) return;

                // Construcción de filtros
                var filtros = new
                {
                    ExactaBuscar = false,
                    TextoBuscar = BuscarRequest?.TextoBuscar ?? "",
                    FiltroPais = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_PAI")?.Seleccion ?? new List<string>(),
                    FiltroOna = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ONA")?.Seleccion ?? new List<string>(),
                    FiltroNorma = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_NOR")?.Seleccion ?? new List<string>(),
                    FiltroEsquema = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ESQ")?.Seleccion ?? new List<string>(),
                    FiltroEstado = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_EST")?.Seleccion ?? new List<string>(),
                    FiltroRecomocimiento = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_REC")?.Seleccion ?? new List<string>()
                };

                // Llamada al servicio
                var result = await Servicio.PsBuscarPalabraAsync(JsonConvert.SerializeObject(filtros), 1, 10);
                ResultadoData = result.Data ?? new List<BuscadorResultadoDataDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CargarResultados: {ex.Message}");
            }
        }

        private void MostrarDetalle(BuscadorResultadoDataDto item)
        {
            Console.WriteLine($"Mostrando detalle del item: {item.DataEsquemaJson}");
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

                var homologaciones = await HomologacionService.GetHomologacionsAsync();
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

        private string? ObtenerDato(BuscadorResultadoDataDto item, int idHomologacion)
        {
            try
            {
                var dato = item.DataEsquemaJson?
                    .FirstOrDefault(f => f.IdHomologacion == idHomologacion)?
                    .Data;

                if (!string.IsNullOrEmpty(dato))
                {
                    // Deserializar el dato si es JSON
                    return System.Text.Json.JsonSerializer.Deserialize<string>(dato);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializando el dato: {ex.Message}");
            }

            return null;
        }


        public class JsonData
        {
            public int IdHomologacion { get; set; }
            public string Data { get; set; } = string.Empty;
        }
    }
}
