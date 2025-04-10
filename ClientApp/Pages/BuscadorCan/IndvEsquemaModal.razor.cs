using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para el modal de esquema.
    /// </summary>
    public partial class IndvEsquemaModal : ComponentBase
    {
        /// <summary>
        /// Resultado de la búsqueda.
        /// </summary>
        [Parameter] public BuscadorResultadoDataDto? resultData { get; set; }

        /// <summary>
        /// Esquema.
        /// </summary>
        [Parameter] public string? esquema { get; set; }

        /// <summary>
        /// Servicio de búsqueda.
        /// </summary>
        [Inject] private IBusquedaService? servicio { get; set; }
        
        /// <summary>
        /// Servicio de JavaScript.
        /// </summary>
        [Inject] private IJSRuntime JS { get; set; } = default!;
        
        /// <summary>
        /// Esquema de cabecera.
        /// </summary>
        private fnEsquemaCabeceraDto? EsquemaCabecera;

        /// <summary>
        /// Columnas de la grilla.
        /// </summary>
        private List<HomologacionDto>? Columnas;

        /// <summary>
        /// Cabeceras.
        /// </summary>
        private List<fnEsquemaCabeceraDto>? Cabeceras;

        /// <summary>
        /// Resultados de la búsqueda.
        /// </summary>
        private List<DataEsquemaDatoBuscar>? resultados;

        /// <summary>
        /// Inicializador de datos.
        /// </summary>
        /// 


        private int? columnaOrdenActualId;
        private bool ordenDescendente = false;
        private Dictionary<int, string> filtros = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                EsquemaCabecera = new fnEsquemaCabeceraDto();
                Columnas = new List<HomologacionDto>();
                esquema = resultData.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == 91)?.Data;

                if (servicio != null)
                {
                    //homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(resultData.IdEsquema ?? 0);
                    EsquemaCabecera = await servicio.FnEsquemaCabeceraAsync(resultData.IdEsquemaData ?? 0);
                    //Cabeceras = (List<fnEsquemaCabeceraDto>?)JsonConvert.DeserializeObject<List<fnEsquemaCabeceraDto>>(EsquemaCabecera?.EsquemaJson ?? "[]");
                    Columnas = (List<HomologacionDto>?)JsonConvert.DeserializeObject<List<HomologacionDto>>(EsquemaCabecera?.EsquemaJson ?? "[]");
                }
                StateHasChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Mostrar datos obtenidos en la grilla.
        /// </summary>
        /// <param name="request">Solicitud de datos.</param>
        /// <returns>Resultado de la grilla.</returns>
        private async Task<GridDataProviderResult<DataEsquemaDatoBuscar>> HomologacionEsquemasDataProvider(GridDataProviderRequest<DataEsquemaDatoBuscar> request)
        {
            try
            {
                if (resultados is null && servicio != null)
                {
                    resultados = await servicio.FnEsquemaDatoBuscarAsync(resultData.IdEsquemaData ?? 0, resultData.Texto);
                }

                IEnumerable<DataEsquemaDatoBuscar> query = resultados ?? new List<DataEsquemaDatoBuscar>();

                // Aplicar filtros manuales por IdHomologacion
                foreach (var filtro in filtros)
                {
                    int idHomologacionFiltro = filtro.Key;
                    string valorFiltro = filtro.Value;

                    if (!string.IsNullOrWhiteSpace(valorFiltro))
                    {
                        query = query.Where(r =>
                            r.DataEsquemaJson != null &&
                            r.DataEsquemaJson.Any(d =>
                                d.IdHomologacion == idHomologacionFiltro &&
                                d.Data != null &&
                                d.Data.Contains(valorFiltro, StringComparison.OrdinalIgnoreCase)));
                    }
                }

                // Aplicar ordenamiento manual
                if (columnaOrdenActualId is not null)
                {
                    int idOrden = columnaOrdenActualId.Value;

                    query = ordenDescendente
                        ? query.OrderByDescending(r =>
                            r.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == idOrden)?.Data)
                        : query.OrderBy(r =>
                            r.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == idOrden)?.Data);
                }

                var resultadoFinal = query.ToList();

                return new GridDataProviderResult<DataEsquemaDatoBuscar>
                {
                    Data = resultadoFinal,
                    TotalCount = resultadoFinal.Count
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en HomologacionEsquemasDataProvider: {ex.Message}");
                return new GridDataProviderResult<DataEsquemaDatoBuscar> { Data = new List<DataEsquemaDatoBuscar>() };
            }
        }


        private async void FiltrarTabla(int idHomologacion, string valor)
        {
            filtros[idHomologacion] = valor;
            StateHasChanged();
        }

        /// <summary>
        /// Obtiene dinámicamente el valor de una propiedad de un objeto.
        /// </summary>
        private object GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
                return string.Empty;

            var prop = obj.GetType().GetProperty(propertyName);
            return prop?.GetValue(obj, null) ?? string.Empty;
        }

        private async Task AplicarOrden(int idHomologacion)
        {
            if (columnaOrdenActualId == idHomologacion)
                ordenDescendente = !ordenDescendente;
            else
            {
                columnaOrdenActualId = idHomologacion;
                ordenDescendente = false;
            }

            StateHasChanged(); // O usa await gridRef?.RefreshDataAsync(); si tienes referencia al grid
        }

        /// <summary>
        /// parseador de formula.
        /// </summary>
        private string ExtraerFormula(string input)
        {
            // Busca la parte dentro de $$ ... $$ y extrae solo la fórmula
            int start = input.IndexOf("$$") + 2;
            int end = input.LastIndexOf("$$");

            if (start >= 2 && end > start)
            {
                return input.Substring(start, end - start).Trim();
            }

            return input; // Si no encuentra, devuelve el mismo dato
        }

        /// <summary>
        /// renderizado de formula luego de la carga de página.
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    Console.WriteLine("📌 Llamando a renderMathJax desde Blazor...");
                    await JS.InvokeVoidAsync("setTimeout", "window.renderMathJax()", 1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"❌ Error al ejecutar renderMathJax: {e.Message}");
                }
            }
        }
    }
}
