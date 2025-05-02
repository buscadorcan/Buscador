using BlazorBootstrap;
using Infractruture.Models;
using Infractruture.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedApp.Dtos;

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

        private Grid<DataEsquemaDatoBuscar>? gridRef;
        private string sortColumn = "Id"; // Columna por defecto
        private bool sortDescending = false; // Orden predeterminado (ascendente)
        private Dictionary<string, string> filtros = new();
        private Dictionary<string, string> operadores = new();
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

                var datosFiltrados = resultados ?? new List<DataEsquemaDatoBuscar>();

                // ✅ Aplicar Filtros Manuales
                if (filtros.Any())
                {
                    foreach (var filtro in filtros)
                    {
                        string columna = filtro.Key;
                        string valorFiltro = filtro.Value;
                        string operador = operadores.ContainsKey(columna) ? operadores[columna] : "contains";

                        if (!string.IsNullOrWhiteSpace(valorFiltro))
                        {
                            datosFiltrados = datosFiltrados
                                .Where(r => r.DataEsquemaJson != null && r.DataEsquemaJson.Any(d =>
                                    d.Data != null &&
                                    d.IdHomologacion == Columnas?.FirstOrDefault(c => c.MostrarWeb == columna)?.IdHomologacion &&
                                    AplicaFiltro(d.Data, valorFiltro, operador)))
                                .ToList();
                        }
                    }
                }

                // ✅ Aplicar Ordenamiento desde request
                if (request.Sorting != null && request.Sorting.Any())
                {
                    foreach (var sort in request.Sorting)
                    {
                        datosFiltrados = sort.SortDirection == SortDirection.Descending
                            ? datosFiltrados.OrderByDescending(d => GetPropertyValue(d, sort.SortString)).ToList()
                            : datosFiltrados.OrderBy(d => GetPropertyValue(d, sort.SortString)).ToList();
                    }
                }

                return new GridDataProviderResult<DataEsquemaDatoBuscar> { Data = datosFiltrados };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en HomologacionEsquemasDataProvider: {ex.Message}");
                return new GridDataProviderResult<DataEsquemaDatoBuscar> { Data = new List<DataEsquemaDatoBuscar>() };
            }
        }

        private async void FiltrarTabla(string columna, string valor)
        {
            filtros[columna] = valor;
            if (gridRef is not null)
                await gridRef.RefreshDataAsync();
        }

        private bool AplicaFiltro(string valor, string filtro, string operador)
        {
            return operador switch
            {
                "starts" => valor.StartsWith(filtro, StringComparison.OrdinalIgnoreCase),
                "ends" => valor.EndsWith(filtro, StringComparison.OrdinalIgnoreCase),
                _ => valor.Contains(filtro, StringComparison.OrdinalIgnoreCase),
            };
        }

        private async void CambiarOperadorFiltro(string columna, string operador)
        {
            operadores[columna] = operador;
            if (gridRef is not null)
                await gridRef.RefreshDataAsync();
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


        /// <summary>
        /// Método que cambia el orden de la columna seleccionada.
        /// </summary>
        private void CambiarOrden(string columna)
        {
            if (sortColumn == columna)
            {
                sortDescending = !sortDescending; // Si es la misma columna, alternar orden
            }
            else
            {
                sortColumn = columna;
                sortDescending = false; // Nueva columna, empezar en ascendente
            }

            StateHasChanged(); // Refrescar UI
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
