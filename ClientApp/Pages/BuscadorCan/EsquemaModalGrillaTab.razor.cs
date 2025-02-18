using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;
using Microsoft.JSInterop; // Para invocar funciones de JavaScript

namespace ClientApp.Pages.BuscadorCan
{
    public partial class EsquemaModalGrillaTab
    {
        [Parameter]
        public int IdEsquema { get; set; }
        [Parameter]
        public int? idONA { get; set; }
        [Parameter]
        public string? VistaFK { get; set; }
        [Parameter]
        public string? VistaPK { get; set; }
        [Parameter]
        public string? Texto { get; set; }
        [Inject]
        private IBusquedaService? servicio { get; set; }
        [Inject]
        private IJSRuntime JS { get; set; } // Para llamar scripts JavaScript
        private HomologacionEsquemaDto? homologacionEsquema;
        private List<HomologacionDto>? Columnas;
        private List<DataHomologacionEsquema>? resultados;
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


        private async Task<GridDataProviderResult<DataHomologacionEsquema>> HomologacionEsquemasDataProvider(GridDataProviderRequest<DataHomologacionEsquema> request)
        {
            if (resultados is null && servicio != null)
            {
                resultados = await servicio.FnHomologacionEsquemaDatoAsync(IdEsquema, VistaFK, idONA ?? 0);
            }

            return await Task.FromResult(request.ApplyTo(resultados ?? []));
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JS.InvokeVoidAsync("renderMathJax"); // Llamamos a MathJax después de cada renderizado
        }
    }
}