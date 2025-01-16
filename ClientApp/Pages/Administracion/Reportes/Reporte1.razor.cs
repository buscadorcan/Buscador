using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientApp.Pages.Administracion.Reportes
{
    public partial class Reporte1
    {
        // Datos para los gráficos
        public List<ChartData> Chart1Data { get; set; } = new List<ChartData>
        {
            new ChartData { Label = "Productos", Value = 10 },
            new ChartData { Label = "Ensayos", Value = 15 }
        };

        public List<ChartData> Chart2Data { get; set; } = new List<ChartData>
        {
            new ChartData { Label = "Producto Acreditado", Value = 5 },
            new ChartData { Label = "Ensayo Retirado", Value = 6 },
            new ChartData { Label = "Ensayo Acreditado", Value = 7 }
        };

        public List<LineChartData> Chart3Data { get; set; } = new List<LineChartData>
        {
            new LineChartData { Fecha = "2024-01-01", Organizaciones = 50 },
            new LineChartData { Fecha = "2024-01-02", Organizaciones = 20 }
        };

        // Datos para los mapas de calor
        public List<MapData> Heatmap1Data { get; set; } = new List<MapData>
        {
            new MapData { Pais = "Ecuador", Organizaciones = 10 },
            new MapData { Pais = "Peru", Organizaciones = 15 }
        };

        public List<MapData> Heatmap2Data { get; set; } = new List<MapData>
        {
            new MapData { Pais = "Ecuador", Organizaciones = 10 },
            new MapData { Pais = "Peru", Organizaciones = 15 },
            new MapData { Pais = "Bolivia", Organizaciones = 16 }
        };

        public List<MapData> Heatmap3Data { get; set; } = new List<MapData>
        {
            new MapData { Pais = "Ecuador", Organizaciones = 5 },
            new MapData { Pais = "Peru", Organizaciones = 6 },
            new MapData { Pais = "Bolivia", Organizaciones = 7 }
        };

        // Método ejecutado después de renderizar el componente
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    await JS.InvokeVoidAsync("initMap", new
                    {
                        chartsData = new[]
                        {
                            Chart1Data.Select(d => new { label = d.Label, value = d.Value }),
                            Chart2Data.Select(d => new { label = d.Label, value = d.Value }),
                            Chart3Data.Select(d => new { label = d.Fecha, value = d.Organizaciones })
                        },
                        mapsData = new
                        {
                            heatmap1 = Heatmap1Data,
                            heatmap2 = Heatmap2Data,
                            heatmap3 = Heatmap3Data
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
            public int Organizaciones { get; set; }
        }

        public class MapData
        {
            public string Pais { get; set; }
            public int Organizaciones { get; set; }
        }
    }
}
