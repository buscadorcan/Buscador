using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using Microsoft.JSInterop; // Para invocar funciones de JavaScript

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para el modal de grilla de esquema.
    /// </summary>
    public partial class EsquemaModalGrillaTab : ComponentBase
    {
        /// <summary>
        /// Servicio de búsqueda.
        /// </summary>
        [Inject] private IBusquedaService? servicio { get; set; }

        /// <summary>
        /// Servicio de JavaScript.
        /// </summary>
        [Inject] private IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// Identificador de esquema.
        /// </summary>
        [Parameter] public int IdEsquema { get; set; }

        /// <summary>
        /// Identificador de ONA.
        /// </summary>
        [Parameter] public int? idONA { get; set; }

        /// <summary>
        /// Identificador de vista secundario
        /// </summary>
        [Parameter] public string? VistaFK { get; set; }

        /// <summary>
        /// Identificador de vista primario
        /// </summary>
        [Parameter] public string? VistaPK { get; set; }

        /// <summary>
        /// Texto de búsqueda.
        /// </summary>
        [Parameter] public string? Texto { get; set; }
        
        /// <summary>
        /// Homologación de esquema.
        /// </summary>
        private HomologacionEsquemaDto? homologacionEsquema;

        /// <summary>
        /// Columnas de la grilla.
        /// </summary>
        private List<HomologacionDto>? Columnas;

        /// <summary>
        /// Resultados de esquemas.
        /// </summary>
        private List<DataHomologacionEsquema>? resultados;

        /// <summary>
        /// Método de inicialización de datos.
        /// </summary>

        private Dictionary<int, string> filtros = new();

        private Grid<DataHomologacionEsquema>? gridRef;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (servicio != null)
                {
                    homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(IdEsquema);
                    Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema?.EsquemaJson ?? "[]")?.OrderBy(c => c.MostrarWebOrden).ToList();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Método para obtener los datos de la grilla.
        /// </summary>
        /// <param name="request">Solicitud de datos.</param>
        /// <returns>Resultado de la solicitud.</returns>

        private async Task<GridDataProviderResult<DataHomologacionEsquema>> HomologacionEsquemasDataProvider(GridDataProviderRequest<DataHomologacionEsquema> request)
        {
            if (resultados is null && servicio != null)
            {
                resultados = await servicio.FnHomologacionEsquemaDatoAsync(IdEsquema, VistaFK, idONA ?? 0);
            }

            IEnumerable<DataHomologacionEsquema> query = resultados ?? new List<DataHomologacionEsquema>();

            // Aplicar filtros manuales (personalizados)
            foreach (var filtro in filtros)
            {
                int idHomologacionFiltro = filtro.Key;
                string valorFiltro = filtro.Value;

                if (!string.IsNullOrEmpty(valorFiltro))
                {
                    query = query.Where(r =>
                        r.DataEsquemaJson != null &&
                        r.DataEsquemaJson.Any(d =>
                            d.IdHomologacion == idHomologacionFiltro &&
                            d.Data != null &&
                            d.Data.Contains(valorFiltro, StringComparison.OrdinalIgnoreCase)));
                }
            }

            // Aplicar ordenamiento (si existe)
            if (request.Sorting?.Any() == true)
            {
                foreach (var sort in request.Sorting)
                {
                    query = sort.SortDirection == SortDirection.Descending
                        ? query.OrderByDescending(sort.SortKeySelector.Compile())
                        : query.OrderBy(sort.SortKeySelector.Compile());
                }
            }

            var resultadoFinal = query.ToList();

            return new GridDataProviderResult<DataHomologacionEsquema>
            {
                Data = resultadoFinal,
                TotalCount = resultadoFinal.Count
            };
        }


        private async void FiltrarTabla(int idHomologacion, string valor)
        {
            filtros[idHomologacion] = valor;

            if (gridRef is not null)
            {
                await gridRef.RefreshDataAsync();
            }
        }

        /// <summary>
        /// Método para extraer la fórmula de un texto.
        /// </summary>
        /// <param name="input">Texto de entrada.</param>
        /// <returns>Fórmula extraída.</returns>
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
        /// Método para ejecutar despues del renderizado.
        /// </summary>
        /// <param name="firstRender">Indicador de primer renderizado.</param>
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