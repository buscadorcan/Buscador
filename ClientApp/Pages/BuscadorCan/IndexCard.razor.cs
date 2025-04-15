using BlazorBootstrap;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.JSInterop;
using SharedApp.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para la página de búsqueda de CAN.
    /// </summary>
    public partial class IndexCard : ComponentBase
    {
        /// <summary>
        /// Servicio para obtener catálogos.
        /// </summary>
        [Inject] public ICatalogosService? iCatalogosService { get; set; }

        /// <summary>
        /// Servicio para obtener la ruta de onas.
        /// </summary>
        [Inject] public IONAService? iONAService { get; set; }

        /// <summary>
        /// Servicio de JavaScript.
        /// </summary>
        [Inject] public IJSRuntime? iJSRuntime { get; set; }

        /// <summary>
        /// Servicio de búsqueda.
        /// </summary>
        [Inject] public IBusquedaService? Servicio { get; set; }

        /// <summary>
        /// Servicio de homologación.
        /// </summary>
        [Inject] public IHomologacionService? HomologacionService { get; set; }

        /// <summary>
        /// Gets or sets the list data dto.
        /// </summary>
        private List<BuscadorResultadoDataDto>? _listDataDto;

        /// <summary>
        /// Gets or sets the list data dto.
        /// </summary>
        [Parameter] public List<BuscadorResultadoDataDto>? ListDataDto
        {
            get => _listDataDto;
            set
            {
                if (!Enumerable.SequenceEqual(_listDataDto ?? new(), value ?? new()))
                {
                    _listDataDto = value;
                    currentPage = 1; // Reiniciar a la página 1 cuando cambia la lista
                    _ = ObtenerCoordenadasYMarcarMapa();
                }
            }
        }

        /// <summary>
        /// Gets or sets the list url data dto.
        /// </summary>
        [Parameter] public Dictionary<int, string>? iconUrls { get; set; }

        /// <summary>
        /// Listado de coordenadas del resultado de la búsquedda.
        /// </summary>
        [Parameter] public List<(string Pais, string Ciudad)> uniqueLocations { get; set; } = new();

        /// <summary>
        /// Listado de etiquetas de la grilla.
        /// </summary>
        [Parameter] public List<VwGrillaDto>? listaEtiquetasGrilla { get; set; }

        /// <summary>
        /// Componente modal.
        /// </summary>
        private Modal modal = default!;
        
        /// <summary>
        /// google maps module
        /// </summary>
        private IJSObjectReference? _googleMapsModule;

        /// <summary>
        /// key google maps
        /// </summary>
        private string ApiKey = "AIzaSyC7NUCEvrqrrQDDDRLK2q0HSqswPxtBVAk";

        /// <summary>
        /// google maps point center
        /// </summary>
        private GoogleMapCenter mapCenter = new(-4, -78);
        /// <summary>
        /// google maps markers
        /// </summary>
        private List<GoogleMapMarker> markers = new();

        /// <summary>
        /// google maps
        /// </summary>
        private GoogleMap? mapa;

        private int currentPage = 1;

        private int pageSize = 10;

        private IEnumerable<BuscadorResultadoDataDto> PagedListDataDto => _listDataDto?.Skip((currentPage - 1) * pageSize).Take(pageSize) ?? new List<BuscadorResultadoDataDto>();
        private int totalPages => (int)Math.Ceiling((_listDataDto?.Count ?? 0) / (double)pageSize);

        private bool CanGoNext => currentPage < totalPages;
        private bool CanGoBack => currentPage > 1;

        /// <summary>
        /// Inicializar vaklores
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            await ObtenerCoordenadasYMarcarMapa();
        }

        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
        private async void MostrarDetalle(BuscadorResultadoDataDto item)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", item);
            modal.Size = ModalSize.ExtraLarge;
            modal.Style = "font-size: 10px !important;";
            await modal.ShowAsync<EsquemaModal>(title: "Información Detallada", parameters: parameters);
        }
        
        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
        private async void showModalOna(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Size = ModalSize.Regular;
            modal.Style = "font-size: 10px !important;";
            await modal.ShowAsync<OnaModal>(title: "Información Organizacion", parameters: parameters);
        }

        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
        /// <param name="resultData"></param>
        private async void showModalOEC(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Style = "font-size: 10px !important;";
            modal.Size = ModalSize.Regular;
            await modal.ShowAsync<OECModal>(title: "Información del OEC", parameters: parameters);
        }

        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
        private async void showModalESQ(BuscadorResultadoDataDto resultData)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("resultData", resultData);
            modal.Style = "font-size: 10px !important;";
            modal.Size = ModalSize.ExtraLarge;
            await modal.ShowAsync<IndvEsquemaModal>(title: "Información Esquema", parameters: parameters);
        }

        /// <summary>
        /// Método para mostrar el resultados en ventana modal
        /// </summary>
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

        /// <summary>
        /// Método para obtener la URL del certificado.
        /// </summary>
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

        /// <summary>
        /// Método para obtener las coordenadas y marcar el mapa.
        /// </summary>
        private async Task ObtenerCoordenadasYMarcarMapa()
        {
            markers.Clear();
            var processedLocations = new HashSet<string>();

            // ✅ Agrupamos las ONAs por ciudad y contamos cuántos OECs distintos hay en cada ciudad
            var agrupacionONAs = ListDataDto?
                .Where(d => d.DataEsquemaJson != null)
                .GroupBy(d => new
                {
                    Pais = d.DataEsquemaJson.FirstOrDefault(f => f.IdHomologacion == 84)?.Data?.Trim(),
                    Ciudad = d.DataEsquemaJson.FirstOrDefault(f => f.IdHomologacion == 85)?.Data?.Trim()
                })
                .Where(g => !string.IsNullOrEmpty(g.Key.Pais) && !string.IsNullOrEmpty(g.Key.Ciudad))
                .ToDictionary(
                    g => $"{g.Key.Pais}-{g.Key.Ciudad}",
                    g => g.Select(x => x.DataEsquemaJson.FirstOrDefault(f => f.IdHomologacion == 81)?.Data?.Trim())
                          .Where(oec => !string.IsNullOrEmpty(oec))
                          .Distinct()
                          .Count() // ✅ Cantidad de OECs diferentes por ciudad
                );

            foreach (var location in agrupacionONAs)
            {
                string locationKey = location.Key;
                string[] locationSplit = locationKey.Split('-');
                string pais = locationSplit[0];
                string ciudad = locationSplit[1];

                if (processedLocations.Contains(locationKey)) continue;

                var coordenadas = await ObtenerCoordenadas(pais, ciudad);
                if (coordenadas != null)
                {
                    string? iconoRuta = await ObtenerIconoONA(pais, ciudad);

                    int cantidadOECs = location.Value;

                    markers.Add(new GoogleMapMarker
                    {
                        Position = new GoogleMapMarkerPosition(coordenadas.Latitude, coordenadas.Longitude),
                        Title = $"{ciudad}, {pais} - {cantidadOECs} OEC",
                        Content = $@"
                        <div style='text-align: center; font-size: 12px;'>                                   
                            <p style='margin: 2px 0; font-weight: bold;'>
                                <span style='font-size: 13px;'>{ciudad}, {pais}</span><br />
                                <span style='color: red; font-weight: bold; font-size: 14px;'>OEC: {cantidadOECs}</span>
                            </p>
                            <img src='{iconoRuta}' width='32' height='32' style='border-radius:50%;' />
                        </div>"
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
        }

        /// <summary>
        /// Método para obtener las coordenadas de un lugar.
        /// </summary>
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

        /// <summary>
        /// Clase para deserializar la respuesta de la API de Google Maps.
        /// </summary>
        private class GeocodeResponse { public GeocodeResult[] Results { get; set; } }

        /// <summary>
        /// Clase para deserializar la ubicación de la respuesta de la API de Google Maps.
        /// </summary>
        private class GeocodeResult { public Geometry Geometry { get; set; } }

        /// <summary>
        /// Clase para deserializar la geometría de la respuesta de la API de Google Maps.
        /// </summary>
        private class Geometry { public Location Location { get; set; } }

        /// <summary>
        /// Clase para deserializar la ubicación de la respuesta de la API de Google Maps.
        /// </summary>
        private class Location { public double Lat { get; set; } public double Lng { get; set; } }

        private async Task<string?> ObtenerIconoONA(string pais, string ciudad)
        {
            try
            {
                // Buscar el objeto que contiene el ID del ONA basado en la ciudad y el país
                var resultData = ListDataDto?
                    .FirstOrDefault(d =>
                        d.DataEsquemaJson?.Any(f => f.IdHomologacion == 84 && f.Data?.Trim() == pais) == true &&
                        d.DataEsquemaJson?.Any(f => f.IdHomologacion == 85 && f.Data?.Trim() == ciudad) == true);

                if (resultData == null || resultData.IdONA == null)
                    return $"{Inicializar.UrlBaseApi}Icono/default-marker.png"; // Icono por defecto

                // Obtener la URL del ícono para este ONA
                var Onas = await iONAService.GetONAsAsync(Convert.ToInt32(resultData.IdONA));
                var jsonRuta = Onas?.UrlIcono;

                if (string.IsNullOrWhiteSpace(jsonRuta))
                    return $"{Inicializar.UrlBaseApi}Icono/default-marker.png"; // Si está vacío, usa icono por defecto

                // Deserializar JSON para obtener la ruta correcta
                var objetoRuta = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(jsonRuta);

                if (objetoRuta != null && objetoRuta.ContainsKey("filePath"))
                {
                    return $"{Inicializar.UrlBaseApi}{objetoRuta["filePath"]}"; // Devolver la ruta del icono
                }

                return $"{Inicializar.UrlBaseApi}Icono/default-marker.png"; // Si algo falla, usar ícono por defecto
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo icono ONA: {ex.Message}");
                return $"{Inicializar.UrlBaseApi}Icono/default-marker.png"; // En caso de error, usar un ícono por defecto
            }
        }

        private void NextPage()
        {
            if (CanGoNext)
                currentPage++;
        }

        private void PrevPage()
        {
            if (CanGoBack)
                currentPage--;
        }


    }
}
