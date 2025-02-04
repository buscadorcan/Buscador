using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using ClientApp.Services.IService;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using System.Reflection;
using ClientApp.Services;
using Microsoft.JSInterop;

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
        [Parameter]
        public bool IsExactSearch { get; set; } = false;

        [Inject]
        public IBusquedaService? Servicio { get; set; }

        [Inject]
        public ICatalogosService? CatalogosService { get; set; }

        [Inject]
        public IHomologacionService? HomologacionService { get; set; }
        [Inject]
        public IONAService? iOnaService { get; set; }
        [Inject]
        public IJSRuntime? iJSRuntime { get; set; }
        // Propiedades para la vista
        public List<BuscadorResultadoDataDto>? ResultadoData { get; private set; } = new List<BuscadorResultadoDataDto>();
        public List<VwGrillaDto>? ListaEtiquetasGrilla { get; private set; } = new List<VwGrillaDto>();

        public bool ModoBuscar { get; set; }
        private int currentPage = 1; // Página actual
        private int totalCount = 0; // Total de registros
        private int pageSize = 10; // Tamaño de página
        public string SearchTerm { get; set; } = ""; // Texto ingresado en el input del buscador
        private Modal modal = default!;
        private Dictionary<int, string> iconUrls = new();
        private OnaDto? OnaDto;
        private bool mostrarMapa = true;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Cargar etiquetas
                if (CatalogosService != null)
                {

                    var ordenPersonalizado = new Dictionary<int, int>
                    {
                        { 84, 1 },
                        { 78, 2 },
                        { 82, 3 }, // Eliminado el duplicado
                        { 83, 4 },
                        { 90, 5 },
                        { 93, 6 },
                        { 81, 7 },
                        { 92, 8 },
                        { 91, 9 }
                    };

                    ListaEtiquetasGrilla = (await CatalogosService.GetHomologacionAsync<List<VwGrillaDto>>("grid/schema"))
                                 ?.OrderBy(x => ordenPersonalizado.ContainsKey(x.IdHomologacion) ? ordenPersonalizado[x.IdHomologacion] : int.MaxValue)
                                 .ToList()
                                 ?? new List<VwGrillaDto>();
                }
                Console.WriteLine($"Error en OnInitializedAsync");
                // Cargar resultados iniciales
                await BuscarPalabraRequest();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en OnInitializedAsync: {ex.Message}");
            }
        }

        public async Task BuscarPalabraRequest()
        {
            await CargarResultados(1, pageSize); // Llamar directamente con la paginación inicial
        }

        private async Task CargarResultados(int pageNumber, int pageSize)
        {
            try
            {
                if (Servicio == null) return;

                // Construcción de filtros
                var filtros = new
                {
                    ExactaBuscar = IsExactSearch,
                    TextoBuscar = SearchTerm,
                    FiltroPais = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_PAI")?.Seleccion ?? new List<string>(),
                    FiltroOna = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ONA")?.Seleccion ?? new List<string>(),
                    FiltroNorma = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_NOR")?.Seleccion ?? new List<string>(),
                    FiltroEsquema = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_ESQ")?.Seleccion ?? new List<string>(),
                    FiltroEstado = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_EST")?.Seleccion ?? new List<string>(),
                    FiltroRecomocimiento = SelectedValues?.FirstOrDefault(c => c.CodigoHomologacion == "KEY_FIL_REC")?.Seleccion ?? new List<string>()
                };

                // Llamada al servicio con paginación
                var result = await Servicio.PsBuscarPalabraAsync(JsonConvert.SerializeObject(filtros), pageNumber, pageSize);

                if (result?.Data != null)
                {
                    ResultadoData = result.Data;
                    totalCount = result.TotalCount; // Guardar el total de registros para la paginación

                    // Prepara las URLs de los íconos para cada ONA
                    foreach (var item in ResultadoData)
                    {
                        if (item.IdONA.HasValue && !iconUrls.ContainsKey(item.IdONA.Value))
                        {
                            iconUrls[item.IdONA.Value] = await getIconUrl(item);
                        }
                    }
                }
                else
                {
                    ResultadoData = new List<BuscadorResultadoDataDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CargarResultados: {ex.Message}");
            }
        }

        private async Task OnPageChange(int pageNumber)
        {
            currentPage = pageNumber; // Actualiza la página actual
            await CargarResultados(pageNumber, pageSize);
        }

        private async void MostrarDetalle(BuscadorResultadoDataDto item)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", item);
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
        private async Task AbrirPdf(BuscadorResultadoDataDto item)
        {
            // Obtener la URL del certificado
            var pdfUrl = await GetPdfUrlFromEsquema(item);

            if (string.IsNullOrWhiteSpace(pdfUrl))
            {
                Console.WriteLine("No se encontró la URL del certificado.");
                return;
            }

            // Llamar a la función JavaScript para abrir la ventana emergente
            await iJSRuntime.InvokeVoidAsync("abrirVentanaPDF", pdfUrl);
        }

        private async Task<string?> GetPdfUrlFromEsquema(BuscadorResultadoDataDto resultData)
        {
            try
            {
                if (resultData.DataEsquemaJson == null || !resultData.DataEsquemaJson.Any())
                    return null;

                var homologaciones = await HomologacionService.GetHomologacionsAsync();
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

        private async Task<string> getIconUrl(BuscadorResultadoDataDto item)
        {
            try
            {
                var idOna = item.IdONA;

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

        private void ToggleMapa()
        {
            mostrarMapa = !mostrarMapa; // Cambia el estado
        }


        public class JsonData
        {
            public int IdHomologacion { get; set; }
            public string Data { get; set; } = string.Empty;
        }
    }
}
