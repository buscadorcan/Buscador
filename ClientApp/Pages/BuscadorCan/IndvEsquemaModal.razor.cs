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
        private HomologacionEsquemaDto? homologacionEsquema;
        private List<HomologacionDto>? Columnas;
        private List<DataEsquemaDatoBuscar>? resultados;

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

        private async Task<GridDataProviderResult<DataEsquemaDatoBuscar>> HomologacionEsquemasDataProvider(GridDataProviderRequest<DataEsquemaDatoBuscar> request)
        {
            if (resultados is null && servicio != null)
            {
                resultados = await servicio.FnEsquemaDatoBuscarAsync(idONA ?? 0, IdEsquema, VistaPK, Texto);
            }

            return await Task.FromResult(request.ApplyTo(resultados ?? []));
        }
    }
}
