using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
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
        private HomologacionEsquemaDto? homologacionEsquema;
        private List<HomologacionDto>? Columnas;
        private List<DataEsquemaDatoBuscar>? resultados;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                esquema = resultData.DataEsquemaJson?.FirstOrDefault(f => f.IdHomologacion == 91)?.Data;
                if (servicio != null)
                {
                    homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(resultData.IdEsquema ?? 0);
                    Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema?.EsquemaJson ?? "[]")?.OrderBy(c => c.MostrarWebOrden).ToList();
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
            if (resultados is null && servicio != null)
            {
                resultados = await servicio.FnEsquemaDatoBuscarAsync(resultData.IdEsquemaData ?? 0, resultData.Texto);
            }

            return await Task.FromResult(request.ApplyTo(resultados ?? []));
        }
    }
}
