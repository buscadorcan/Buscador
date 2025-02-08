using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Reportes
{
    public partial class ReporteRead
    {
        [Inject]
        private IReporteService? iReporteService { get; set; }

        // Datos para los gráficos
        public List<ChartData> Chart1Data { get; set; } = new List<ChartData>();
        public List<ChartData> Chart2Data { get; set; } = new List<ChartData>();
        public List<ChartData> Chart3Data { get; set; } = new List<ChartData>();
        public List<LineChartData> Chart4Data { get; set; } = new List<LineChartData>();

        // Datos para los mapas de calor
        public List<MapData> Heatmap1Data { get; set; } = new List<MapData>();

        //titulos
        public string Titulo_vw_ProfesionalCalificado { get; set; } = "";
        public string Titulo_vw_ProfesionalOna { get; set; } = "";
        public string Titulo_vw_ProfesionalEsquema { get; set; } = "";
        public string Titulo_vw_ProfesionalFecha { get; set; } = "";
        public string Titulo_vw_CalificaUbicacion { get; set; } = "";

        // Método ejecutado después de renderizar el componente
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    //var listaVwProfesionalCalificado = await iReporteService.GetVwProfesionalCalificadoAsync<List<VwProfesionalCalificadoDto>>("profesional-calificado");
                    //Titulo_vw_ProfesionalCalificado = (await iReporteService.findByVista("vw_ProfesionalCalificado"))?.MostrarWeb ?? "";
                    //foreach (var item in listaVwProfesionalCalificado)
                    //{
                    //    Chart1Data.Add(new ChartData {Label = item.Calificacion, Value = item.Profesionales});
                    //}

                    //var listaVwProfesionalOna = await iReporteService.GetVwProfesionalOnaAsync<List<VwProfesionalOnaDto>>("profesional-ona");
                    //Titulo_vw_ProfesionalOna = (await iReporteService.findByVista("vw_ProfesionalOna"))?.MostrarWeb ?? "";
                    //foreach (var item in listaVwProfesionalOna)
                    //{
                    //    Chart2Data.Add(new ChartData {Label = item.Ona, Value = item.Profesionales});
                    //}

                    //var listaVwProfesionalEsquema = await iReporteService.GetVwProfesionalEsquemaAsync<List<VwProfesionalEsquemaDto>>("profesional-esquema");
                    //Titulo_vw_ProfesionalEsquema = (await iReporteService.findByVista("vw_ProfesionalEsquema"))?.MostrarWeb ?? "";
                    //foreach (var item in listaVwProfesionalEsquema)
                    //{
                    //    Chart3Data.Add(new ChartData { Label = item.Esquema, Value = item.Profesionales });
                    //}

                    //var listaVwProfesionalFecha = await iReporteService.GetVwProfesionalFechaAsync<List<VwProfesionalFechaDto>>("profesional-fecha");
                    //Titulo_vw_ProfesionalFecha = (await iReporteService.findByVista("vw_ProfesionalFecha"))?.MostrarWeb ?? "";
                    //foreach (var item in listaVwProfesionalFecha)
                    //{
                    //    Chart4Data.Add(new LineChartData { Fecha = item.Fecha, Organizacion = item.Profesionales });
                    //}

                    var listaVwCalificaUbicacion = await iReporteService.GetVwOecPaisAsync<List<VwCalificaUbicacionDto>>("califica-ubicacion");
                    Titulo_vw_CalificaUbicacion = (await iReporteService.findByVista("vw_CalificaUbicacion"))?.MostrarWeb ?? "";
                    foreach (var item in listaVwCalificaUbicacion)
                    {
                        Heatmap1Data.Add(new MapData { Pais = item.Pais, Organizacion = item.Calificados });
                    }

                    StateHasChanged();
                    await JS.InvokeVoidAsync("initMap", new
                    {
                        //chartsData = new[]
                        //{
                        //    Chart1Data.Select(d => new { label = d.Label, value = d.Value }),
                        //    Chart2Data.Select(d => new { label = d.Label, value = d.Value }),
                        //    Chart3Data.Select(d => new { label = d.Label, value = d.Value }),
                        //    Chart4Data.Select(d => new { label = d.Fecha, value = d.Organizacion })

                        //},
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
        }
    }
}
