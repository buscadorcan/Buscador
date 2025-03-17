using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class IndvEsquemaModal
    {
        [Parameter]
        public BuscadorResultadoDataDto? resultData { get; set; }
        [Parameter]
        public string? esquema { get; set; }

        [Inject]
        private IBusquedaService? servicio { get; set; }
        private IJSRuntime JS { get; set; } // Para llamar scripts JavaScript
        private HomologacionEsquemaDto? homologacionEsquema;
        private fnEsquemaCabeceraDto? EsquemaCabecera;
        private List<HomologacionDto>? Columnas;
        private List<fnEsquemaCabeceraDto>? Cabeceras;
        private List<DataEsquemaDatoBuscar>? resultados;

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

                return await Task.FromResult(request.ApplyTo(resultados ?? []));
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

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
