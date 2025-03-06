using ClientApp.Helpers;
using ClientApp.Services.IService;
using Microsoft.AspNetCore.Components;
using SharedApp.Models.Dtos;

namespace ClientApp.Pages.BuscadorCan
{
    /// <summary>
    /// Componente parcial para el modal de ONA.
    /// </summary>
    public partial class OnaModal : ComponentBase
    {
        /// <summary>
        /// Servicio de catálogos.
        /// </summary>
        [Inject] private ICatalogosService iCatalogoService { get; set; } = default!;

        /// <summary>
        /// Resultado de datos de búsqueda.
        /// </summary>
        [Parameter] public BuscadorResultadoDataDto ResultData { get; set; } = default!;

        /// <summary>
        /// ONA seleccionado.
        /// </summary>
        private vwONADto? onaSeleccionado;

        /// <summary>
        /// Indicador de carga.
        /// </summary>
        private bool loading = true;

        /// <summary>
        /// Url base de la API.
        /// </summary>
        private string url = Inicializar.UrlBaseApi;

        /// <summary>
        /// Método de inicialización de datos.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                var listaOnas = await iCatalogoService.GetvwOnaAsync();
                onaSeleccionado = listaOnas.FirstOrDefault(ona => ona.IdONA == ResultData.IdONA);
                if (!string.IsNullOrEmpty(onaSeleccionado?.UrlIcono) && onaSeleccionado.UrlIcono.Contains("filePath"))
                {
                    var iconoJson = System.Text.Json.JsonDocument.Parse(onaSeleccionado.UrlIcono);
                    onaSeleccionado.UrlIcono = url + iconoJson.RootElement.GetProperty("filePath").GetString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el ONA: {ex.Message}");
            }
            finally
            {
                loading = false;
            }
        }
    }
}
