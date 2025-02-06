using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class OECModal
    {
        [Parameter]
        public BuscadorResultadoDataDto ResultData { get; set; } = default!;
        private vwEsquemaOrganizaDto? onaSeleccionado;
        private bool loading = true;

        [Inject]
        private ICatalogosService iCatalogoService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                //var listaOnas = await iCatalogoService.GetvwOnaAsync();
                var listaOnas = await iCatalogoService.GetvwEsquemaOrganizaAsync();
                onaSeleccionado = listaOnas.FirstOrDefault(ona => ona.IdEsquemaData == ResultData.IdEsquemaData);

            
                if (!string.IsNullOrEmpty(onaSeleccionado?.ONAUrlIcono) && onaSeleccionado.ONAUrlIcono.Contains("filePath"))
                {
                    var iconoJson = System.Text.Json.JsonDocument.Parse(onaSeleccionado.ONAUrlIcono);
                    onaSeleccionado.ONAUrlIcono = iconoJson.RootElement.GetProperty("filePath").GetString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el OEC: {ex.Message}");
            }
            finally
            {
                loading = false;
            }
        }
    }
}
