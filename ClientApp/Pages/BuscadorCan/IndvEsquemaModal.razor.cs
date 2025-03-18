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
                    // 🔥 Reiniciar la lista antes de cargar nuevos datos
                    resultados = new List<DataEsquemaDatoBuscar>();
                    resultados = await servicio.FnEsquemaDatoBuscarAsync(resultData.IdEsquemaData ?? 0, resultData.Texto);
                }

                await JS.InvokeVoidAsync("renderMathJax");
                return await Task.FromResult(request.ApplyTo(resultados ?? []));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new GridDataProviderResult<DataEsquemaDatoBuscar>();
            }

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
