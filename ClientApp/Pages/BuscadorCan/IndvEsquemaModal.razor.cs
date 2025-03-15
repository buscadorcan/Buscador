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

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                Console.WriteLine("♻️ Refrescando datos en el modal con nuevo `resultData`");

                // 🔄 Reiniciar los datos antes de actualizar
                EsquemaCabecera = null;
                Columnas = new List<HomologacionDto>();
                resultados = null;
                esquema = resultData?.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == 91)?.Data;

                if (servicio != null && resultData != null)
                {
                    // 🔄 Forzar actualización de la UI antes de cargar nuevos datos
                    StateHasChanged();
                    await Task.Delay(50); // 🔄 Pequeña espera para permitir que Blazor detecte los cambios

                    // Cargar nuevos datos
                    EsquemaCabecera = await servicio.FnEsquemaCabeceraAsync(resultData.IdEsquemaData ?? 0);
                    Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(EsquemaCabecera?.EsquemaJson ?? "[]") ?? new List<HomologacionDto>();
                    resultados = await servicio.FnEsquemaDatoBuscarAsync(resultData.IdEsquemaData ?? 0, resultData.Texto);
                }

                StateHasChanged(); // 🔄 Forzar actualización de la UI con los nuevos datos
            }
            catch (Exception e)
            {
                Console.WriteLine($"❌ Error en OnParametersSetAsync: {e.Message}");
            }
        }



        private async Task<GridDataProviderResult<DataEsquemaDatoBuscar>> HomologacionEsquemasDataProvider(GridDataProviderRequest<DataEsquemaDatoBuscar> request)
        {
            try
            {
               
                if (resultados is null && servicio != null)
                {
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
