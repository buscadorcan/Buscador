using BlazorBootstrap;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Org.BouncyCastle.Crmf;
using SharedApp.Models.Dtos;
using System.Net.Http.Json;

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
        [Parameter]
        public List<(string Pais, string Ciudad)> uniqueLocations { get; set; } = new();

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
        [Inject]
        private HttpClient Http { get; set; }
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
        private IJSObjectReference? _googleMapsModule;
        private string ApiKey = "AIzaSyC7NUCEvrqrrQDDDRLK2q0HSqswPxtBVAk";
        private GoogleMapCenter mapCenter = new(0, 0);
        private List<GoogleMapMarker> markers = new();
        private GoogleMap? mapa;
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
                StateHasChanged();
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
            StateHasChanged();
            await ObtenerCoordenadasYMarcarMapa();
            if (mapa != null)
            {
                await mapa.RefreshAsync();
            }

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
                            var iconUrl = await getIconUrl(item);
                            iconUrls[item.IdONA.Value] = $"{Inicializar.UrlBaseApi.TrimEnd('/')}/{iconUrl.TrimStart('/')}";
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

        private async void MostrarDetalle(BuscadorResultadoDataDto item)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", item);
            modal.Size = ModalSize.ExtraLarge;
            modal.Style = "font-family: 'Inter-Medium', Helvetica, sans-serif !important; font-size: 10px !important;";
            await modal.ShowAsync<EsquemaModal>(title: "Información Detallada", parameters: parameters);
        }

        private async void showModalOna(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Size = ModalSize.Regular;
            modal.Style = "font-family: 'Inter-Medium', Helvetica, sans-serif !important; font-size: 10px !important;";
            await modal.ShowAsync<OnaModal>(title: "Información Organizacion", parameters: parameters);
        }

        private async void showModalOEC(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Style = "font-family: 'Inter-Medium', Helvetica, sans-serif !important; font-size: 10px !important;";
            modal.Size = ModalSize.Regular;
            await modal.ShowAsync<OECModal>(title: "Información del OEC", parameters: parameters);
        }

        private async void showModalESQ(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Style = "font-family: 'Inter-Medium', Helvetica, sans-serif !important; font-size: 10px !important;";
            modal.Size = ModalSize.ExtraLarge;
            await modal.ShowAsync<IndvEsquemaModal>(title: "Información Esquema", parameters: parameters);
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

        private async Task ObtenerCoordenadasYMarcarMapa()
        {
            markers.Clear(); // Limpiar los marcadores previos
            var processedLocations = new HashSet<string>();

            uniqueLocations = ResultadoData?
                .Select(d =>
                {
                    var pais = d.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == 84)?.Data?.Trim();
                    var ciudad = d.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == 85)?.Data?.Trim();
                    return (!string.IsNullOrEmpty(pais) && !string.IsNullOrEmpty(ciudad)) ? (pais, ciudad) : default;
                })
                .Where(loc => loc != default)
                .Distinct()
                .ToList() ?? new List<(string, string)>();

            foreach (var location in uniqueLocations)
            {
                var locationKey = $"{location.Pais}-{location.Ciudad}";
                if (processedLocations.Contains(locationKey)) continue;

                var coordenadas = await ObtenerCoordenadas(location.Pais, location.Ciudad);
                if (coordenadas != null)
                {
                    markers.Add(new GoogleMapMarker
                    {
                        Position = new GoogleMapMarkerPosition(coordenadas.Latitude, coordenadas.Longitude),
                        Title = $"{location.Ciudad}, {location.Pais}",
                        PinElement = new PinElement { BorderColor = "red" }
                    });

                    processedLocations.Add(locationKey);
                    if (markers.Count == 1)
                    {
                        mapCenter = coordenadas;
                    }
                }
            }

            if (mapa != null)
            {
                await mapa.RefreshAsync();
            }

            markers = new List<GoogleMapMarker>(markers);
            StateHasChanged();
        }


        private async Task<GoogleMapCenter?> ObtenerCoordenadas(string pais, string ciudad)
        {
            try
            {
                var response = await Servicio.ObtenerCoordenadasAsync(pais, ciudad);

                if (response?.Results?.Length > 0)
                {
                    var location = response.Results[0].Geometry.Location;
                    return new GoogleMapCenter(location.Lat, location.Lng);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo coordenadas desde el servicio: {ex.Message}");
            }
            return null;
        }



        // Clases para deserializar la respuesta de la API de Google
        private class GeocodeResponse { public GeocodeResult[] Results { get; set; } }
        private class GeocodeResult { public Geometry Geometry { get; set; } }
        private class Geometry { public Location Location { get; set; } }
        private class Location { public double Lat { get; set; } public double Lng { get; set; } }


        public class JsonData
        {
            public int IdHomologacion { get; set; }
            public string Data { get; set; } = string.Empty;
        }
    }
}
