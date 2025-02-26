using Blazored.LocalStorage;
using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.Administracion.Reportes
{
    public partial class ReporteOna
    {
        [Inject]
        private IReporteService? iReporteService { get; set; }


        // Datos para los gráficos
        public List<ChartData> Chart1Data { get; set; } = new List<ChartData>();
        public List<ChartData> Chart2Data { get; set; } = new List<ChartData>();
        public List<ChartData> Chart3Data { get; set; } = new List<ChartData>();

        //titulos
        public string Titulo_vw_OrganismoRegistrado { get; set; } = "";
        public string Titulo_vw_OrganizacionEsquema { get; set; } = "";
        public string Titulo_vw_OrganismoActividad { get; set; } = "";


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
            objEventTracking.NombrePagina = "/reporteona";
            objEventTracking.NombreAccion = "OnAfterRenderAsync";
            objEventTracking.NombreControl = "reporteona";
            objEventTracking.NombreUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Local);
            objEventTracking.TipoUsuario = await iLocalStorageService.GetItemAsync<string>(Inicializar.Datos_Usuario_Codigo_Rol_Local);
            objEventTracking.ParametroJson = "{}";
            objEventTracking.UbicacionJson = "";
            await iBusquedaService.AddEventTrackingAsync(objEventTracking);

            if (firstRender)
            {
                try
                {
                    // Cargar datos desde el servicio
                    var listaVwOrganismoRegistrado = await iReporteService.GetVwOrganismoRegistradoAsync<List<VwOrganismoRegistradoDto>>("organismo-registrado");
                    Titulo_vw_OrganismoRegistrado = (await iReporteService.findByVista("vw_OrganismoRegistrado"))?.MostrarWeb ?? "";
                    if (listaVwOrganismoRegistrado != null)
                    {
                        foreach (var item in listaVwOrganismoRegistrado)
                        {
                            Chart1Data.Add(new ChartData { Label = item.Fecha, Value = item.Profesionales });
                        }
                    }

                    var listaVwOrganizacionEsquema = await iReporteService.GetVwOrganizacionEsquemaAsync<List<VwOrganizacionEsquemaDto>>("organizacion-esquema");
                    Titulo_vw_OrganizacionEsquema = (await iReporteService.findByVista("vw_OrganizacionEsquema"))?.MostrarWeb ?? "";
                    if (listaVwOrganizacionEsquema != null)
                    {
                        foreach (var item in listaVwOrganizacionEsquema)
                        {
                            Chart2Data.Add(new ChartData { Label = item.Esquema, Value = item.Organizacion });
                        }
                    }

                    var listaVwOrganismoActividad = await iReporteService.GetVwOrganismoActividadAsync<List<VwOrganismoActividadDto>>("organismo-actividad");
                    Titulo_vw_OrganismoActividad = (await iReporteService.findByVista("vw_OrganismoActividad"))?.MostrarWeb ?? "";
                    if (listaVwOrganismoActividad != null)
                    {
                        foreach (var item in listaVwOrganismoActividad)
                        {
                            Chart3Data.Add(new ChartData { Label = item.Organismos, Value = item.Consultas });
                        }
                    }

                    StateHasChanged();
                    // Verificar datos cargados antes de invocar el script
                    if (Chart1Data.Any() || Chart2Data.Any() || Chart3Data.Any())
                    {
                        await JS.InvokeVoidAsync("initMap", new
                        {
                            chartsData = new[]
                            {
                                Chart1Data.Select(d => new { label = d.Label, value = d.Value }),
                                Chart2Data.Select(d => new { label = d.Label, value = d.Value }),
                                Chart3Data.Select(d => new { label = d.Label, value = d.Value })
                            }
                        });
                    }
                    else
                    {
                        Console.WriteLine("No se cargaron datos para los gráficos.");
                    }
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
            public string Label { get; set; } = string.Empty; // Aseguramos inicialización
            public int Value { get; set; }
        }
    }
}
