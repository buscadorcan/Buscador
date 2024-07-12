using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class EsquemaModalGrillaTab
    {
        [Parameter]
        public int IdHomologacionEsquema { get; set; }
        [Parameter]
        public int IdDataLakeOrganizacion { get; set; }
        [Inject]
        private IBusquedaService? servicio { get; set; }
        private HomologacionEsquemaDto? homologacionEsquema;
        private List<HomologacionDto>? Columnas;
        private List<DataHomologacionEsquema>? resultados;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (servicio != null)
                {
                    homologacionEsquema = await servicio.FnHomologacionEsquemaAsync(IdHomologacionEsquema);
                    Columnas = JsonConvert.DeserializeObject<List<HomologacionDto>>(homologacionEsquema.EsquemaJson ?? "[]")?.OrderBy(c => c.MostrarWebOrden).ToList();
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
                resultados = await servicio.FnHomologacionEsquemaDatoAsync(IdHomologacionEsquema, IdDataLakeOrganizacion);
            }

            return await Task.FromResult(request.ApplyTo(resultados ?? []));
        }
    }
}