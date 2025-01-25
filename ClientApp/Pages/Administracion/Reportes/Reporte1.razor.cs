using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Reportes
{
    public partial class Reporte1
    {
        [Inject]
        private IReporteService? iReporteService { get; set; }


        // Datos para los gráficos
        public List<ChartData> Chart1Data { get; set; } = new List<ChartData>();
        public List<ChartData> Chart2Data { get; set; } = new List<ChartData>();
        public List<LineChartData> Chart3Data { get; set; } = new List<LineChartData>();

        // Datos para los mapas de calor
        public List<MapData> Heatmap1Data { get; set; } = new List<MapData>();
        public List<MapData> Heatmap2Data { get; set; } = new List<MapData>();
        public List<MapData> Heatmap3Data { get; set; } = new List<MapData>();

        // Método ejecutado después de renderizar el componente
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    var listaVwAcreditacionEsquema = await iReporteService.GetVwAcreditacionEsquemaAsync<List<VwAcreditacionEsquemaDto>>("acreditacion-esquema");
                    foreach (var item in listaVwAcreditacionEsquema)
                    {
                        Chart1Data.Add(new ChartData { Label = item.Esquema, Value = item.Organizacion });
                    }

                    var listaVwEstadoEsquema = await iReporteService.GetVwEstadoEsquemaAsync<List<VwEstadoEsquemaDto>>("estado-esquema");
                    foreach (var item in listaVwEstadoEsquema)
                    {
                        Chart2Data.Add(new ChartData { Label = item.Esquema + " " + item.Estado, Value = item.Organizacion });
                    }

                    var listaVwOecFecha = await iReporteService.GetVwOecFechaAsync<List<VwOecFechaDto>>("oec-fecha");
                    foreach (var item in listaVwOecFecha)
                    {
                        Chart3Data.Add(new LineChartData { Fecha = item.Fecha, Organizacion = item.Organizacion });
                    }

                    var listaVwOecPais = await iReporteService.GetVwOecPaisAsync<List<VwOecPaisDto>>("oec-pais");
                    foreach (var item in listaVwOecPais)
                    {
                        Heatmap1Data.Add(new MapData { Pais = item.Pais, Organizacion = item.Organizacion, Esquema = "" }); // Incluye Esquema
                    }

                    var listaVwAcreditacionOna = await iReporteService.GetVwAcreditacionOnaAsync<List<VwAcreditacionOnaDto>>("acreditacion-ona");
                    foreach (var item in listaVwAcreditacionOna)
                    {
                        Heatmap2Data.Add(new MapData { Pais = item.Pais + "-" + item.ONA, Organizacion = item.Organizacion, Esquema = item.ONA }); // Incluye Esquema
                    }

                    var listaVwEsquemaPais = await iReporteService.GetVwEsquemaPaisAsync<List<VwEsquemaPaisDto>>("esquema-pais");
                    foreach (var item in listaVwEsquemaPais)
                    {
                        Heatmap3Data.Add(new MapData { Pais = item.Pais, Organizacion = item.Organizacion, Esquema = item.Esquema }); // Incluye Esquema
                    }

                    await JS.InvokeVoidAsync("initMap", new
                    {
                        chartsData = new[]
                        {
                            Chart1Data.Select(d => new { label = d.Label, value = d.Value }),
                            Chart2Data.Select(d => new { label = d.Label, value = d.Value }),
                            Chart3Data.Select(d => new { label = d.Fecha, value = d.Organizacion })
                        },
                        mapsData = new
                        {
                            heatmap1 = Heatmap1Data.Select(d => new { d.Pais, d.Organizacion, d.Esquema }),
                            heatmap2 = Heatmap2Data.Select(d => new { d.Pais, d.Organizacion, d.Esquema }),
                            heatmap3 = Heatmap3Data.Select(d => new { d.Pais, d.Organizacion, d.Esquema })
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
        public class ChartData
        {
            public string Label { get; set; }
            public int Value { get; set; }
        }

        public class LineChartData
        {
            public string Fecha { get; set; }
            public int Organizacion { get; set; }
        }

        public class MapData
        {
            public string Pais { get; set; }
            public int Organizacion { get; set; }
            public string Esquema { get; set; } // Nuevo campo
        }
    }
}
