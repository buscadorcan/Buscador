using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    public partial class OECModal
    {
        [Parameter]
        public BuscadorResultadoDataDto ResultData { get; set; } = default!;
        private vwONADto? onaSeleccionado;
        private bool loading = true;
        private string? datoOEC;

        [Inject]
        private ICatalogosService iCatalogoService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                var listaOnas = await iCatalogoService.GetvwOnaAsync();
                onaSeleccionado = listaOnas.FirstOrDefault(ona => ona.IdONA == ResultData.IdONA);

                // Extraer el dato de DataEsquemaJson donde IdHomologacion == 81
                datoOEC = ResultData?.DataEsquemaJson?
                    .FirstOrDefault(f => f.IdHomologacion == 81)?.Data;


                //if (!string.IsNullOrEmpty(onaSeleccionado?.UrlIcono) && onaSeleccionado.UrlIcono.Contains("filePath"))
                //{
                //    var iconoJson = System.Text.Json.JsonDocument.Parse(onaSeleccionado.UrlIcono);
                //    onaSeleccionado.UrlIcono = iconoJson.RootElement.GetProperty("filePath").GetString();
                //}
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
