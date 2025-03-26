using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Reportes
{
    public partial class ReporteCan
    {
        [Inject]
        private IReporteService? iReporteService { get; set; }

        // Datos para los gráficos
        public List<LineChartData> Chart1Data { get; set; } = new List<LineChartData>();
        public List<ChartData> Chart2Data { get; set; } = new List<ChartData>();
        public List<LineChartData> Chart3Data { get; set; } = new List<LineChartData>();


        // Datos para los mapas de calor
        public List<MapData> Heatmap1Data { get; set; } = new List<MapData>();

        public string Titulo_vw_BusquedaFecha { get; set; } = "";
        public string Titulo_vw_BusquedaFiltro { get; set; } = "";
        public string Titulo_vw_BusquedaUbicacion { get; set; } = "";
        public string Titulo_vw_ActualizacionONA { get; set; } = "";


        [Inject]
        ILocalStorageService iLocalStorageService { get; set; }
        /// <summary>
        /// Servicio de búsqueda y registro de eventos.
        /// </summary>
        [Inject]
        private IBusquedaService iBusquedaService { get; set; }
        private EventTrackingDto objEventTracking { get; set; } = new();
        // Método ejecutado después de renderizar el componente
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            objEventTracking.CodigoHomologacionMenu = "/reportecan";
            objEventTracking.NombreAccion = "OnAfterRenderAsync";
            objEventTracking.NombreControl = "reportecan";
            objEventTracking.idUsuario = await iLocalStorageService.GetItemAsync<int>(Inicializar.Datos_Usuario_Local);
            objEventTracking.CodigoHomologacionRol = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (firstRender)
            {
                try
                {
                    // Cargar datos para Chart1
                    var listaVwBusquedaFecha = await iReporteService.GetVwBusquedaFechaAsync<List<VwBusquedaFechaDto>>("busqueda-fecha");
                    Titulo_vw_BusquedaFecha = (await iReporteService.findByVista("vw_BusquedaFecha"))?.MostrarWeb ?? "";
                    foreach (var item in listaVwBusquedaFecha)
                    {
                        Chart1Data.Add(new LineChartData { Fecha = item.Fecha, Organizacion = item.Busqueda });
                    }

                    // Cargar datos para Chart2
                    var listaVwBusquedaFiltro = await iReporteService.GetVwBusquedaFiltroAsync<List<VwBusquedaFiltroDto>>("busqueda-filtro");
                    Titulo_vw_BusquedaFiltro = (await iReporteService.findByVista("vw_BusquedaFiltro"))?.MostrarWeb ?? "";
                    foreach (var item in listaVwBusquedaFiltro)
                    {
                        Chart2Data.Add(new ChartData { Label = item.FiltroPor, Value = item.Busqueda });
                    }

                    // Cargar datos para Heatmap
                    var listaVwBusquedaUbicacion = await iReporteService.GetVwBusquedaUbicacionAsync<List<VwBusquedaUbicacionDto>>("busqueda-ubicacion");
                    Titulo_vw_BusquedaUbicacion = (await iReporteService.findByVista("vw_BusquedaUbicacion"))?.MostrarWeb ?? "";
                    foreach (var item in listaVwBusquedaUbicacion)
                    {
                        Heatmap1Data.Add(new MapData { Pais = item.Pais, Ciudad = item.Ciudad, Organizacion = item.Busqueda });
                    }

                    // Cargar datos para Chart3 (incluyendo ONA)
                    var listaVwActualizacionONA = await iReporteService.GetVwActualizacionONAAsync<List<VwActualizacionONADto>>("actualizacion-ona");
                    Titulo_vw_ActualizacionONA = (await iReporteService.findByVista("vw_ActualizacionONA"))?.MostrarWeb ?? "";
                    foreach (var item in listaVwActualizacionONA)
                    {
                        Chart3Data.Add(new LineChartData
                        {
                            Fecha = item.Fecha,
                            Organizacion = item.Actualizaciones,
                            ONA = item.ONA // Nuevo campo ONA
                        });
                    }

                    StateHasChanged();
                    // Enviar datos a JavaScript
                    await JS.InvokeVoidAsync("initMap", new
                    {
                        chartsData = new[]
                        {
                        Chart1Data.Select(d => new { label = d.Fecha, value = d.Organizacion, ona = (string?)null }), // Añade "ona" como null
                        Chart2Data.Select(d => new { label = d.Label, value = d.Value, ona = (string?)null }),       // Añade "ona" como null
                        Chart3Data.Select(d => new { label = d.Fecha, value = d.Organizacion, ona = d.ONA })        // Incluye "ona" en Chart3Data
                    },
                        mapsData = new
                        {
                            heatmap1 = Heatmap1Data
                        }
                    });
                }
                catch (JSException jsEx)
                {
                    Console.WriteLine($"Error al inicializar el mapa en JavaScript: {jsEx.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inesperado: {ex.Message}");
                }
            }
        }

        // Modelos para datos
        public class LineChartData
        {
            public string Fecha { get; set; }
            public int Organizacion { get; set; }
            public string ONA { get; set; } // Nuevo campo agregado
        }
        public class ChartData
        {
            public string Label { get; set; }
            public int Value { get; set; }
        }

        //public class LineChartData
        //{
        //    public string Fecha { get; set; }
        //    public int Organizacion { get; set; }
        //}

        public class MapData
        {
            public string Pais { get; set; }
            public string Ciudad { get; set; }
            public int Organizacion { get; set; }

        }
    }
}
