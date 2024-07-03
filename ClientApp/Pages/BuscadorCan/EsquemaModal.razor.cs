using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class EsquemaModal
    {
        Tabs tabs = default!;
        [Parameter]
        public DataHomologacionEsquema? dataLake { get; set; }
        [Inject]
        private IBusquedaService? servicio { get; set; }
        private List<HomologacionEsquema>? listaEsquemas = new List<HomologacionEsquema>();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (servicio != null) {
                    listaEsquemas = await servicio.FnHomologacionEsquemaTodoAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}