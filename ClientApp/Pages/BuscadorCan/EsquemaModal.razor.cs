using BlazorBootstrap;
using ClientApp.Models;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class EsquemaModal
    {
        Tabs tabs = default!;
        [Parameter]
        public BuscadorResultadoDataDto? resultData { get; set; }
        [Inject]
        private IBusquedaService? servicio { get; set; }
        private List<HomologacionEsquemaDto>? listaEsquemas = new List<HomologacionEsquemaDto>();
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (servicio != null) {
                    listaEsquemas = await servicio.FnHomologacionEsquemaTodoAsync(resultData?.VistaFK, resultData?.IdONA ?? 0);
                    Console.WriteLine($"Error en listaEsquemas:");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}